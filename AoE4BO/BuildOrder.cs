﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AoE4BO
{
    public class BuildOrder
    {
        public BuildOrderStep FirstBuildOrderStep { get; set; }
        public BuildOrderStep CurrentBuildOrderStep { get; set; }
        private Timer _timerUpdate;
        private Stopwatch _stopwatch;
        public delegate void MyEventHandler(object source, EventArgs e);
        public event MyEventHandler StepRefresh;
        private System.Media.SoundPlayer _soundPlayer;

        public BuildOrder(string buildOrderString)
        {
            Global.BoState = BoState.Idle;
            ParseBuildOrderString(buildOrderString);
            _soundPlayer = new System.Media.SoundPlayer(@"notification.wav");
        }

        public void Start()
        {
            // reset game values so no build order step get skipped
            Global.GameData.Supply = 0;
            Global.GameData.SupplyCap = 0;
            Global.GameData.Food = 0;
            Global.GameData.Wood = 0;
            Global.GameData.Gold = 0;
            Global.GameData.Stone = 0;

            Global.BoState = BoState.Running;
            _stopwatch = new Stopwatch();
            _timerUpdate = new Timer(new TimerCallback(Update), null, 100, 100);
        }

        public void Stop()
        {
            Global.BoState = BoState.Idle;
            _timerUpdate.Dispose();
            _stopwatch.Stop();
        }

        public void Restart()
        {
            Stop();
            ResetBuildOrderSteps();
            Start();
        }

        private void Update(object state)
        {
            // get delta time
            float deltaTime = _stopwatch.ElapsedMilliseconds / 1000f;
            _stopwatch.Restart();

            if (CurrentBuildOrderStep.NextBuildOrderStep == null)
                return;

            // check if current build order step forfill all requirements
            if (CurrentBuildOrderStep.NextBuildOrderStep.RequirementsMet(
                Global.GameData.Supply, Global.GameData.SupplyCap,
                Global.GameData.Food, Global.GameData.Wood,
                Global.GameData.Gold, Global.GameData.Stone))
            {
                NextStep();

                if (Global.Settings.PlaySound)
                    _soundPlayer.Play();
            }

            // adding delta time to next build order step to check time requirement
            if (CurrentBuildOrderStep.NextBuildOrderStep != null)
                CurrentBuildOrderStep.NextBuildOrderStep.TimeActive += deltaTime;
        }

        public void NextStep()
        {
            if (CurrentBuildOrderStep.NextBuildOrderStep == null)
                return;

            // set the next build order step
            CurrentBuildOrderStep = CurrentBuildOrderStep.NextBuildOrderStep;
            CurrentBuildOrderStep.TimeActive = 0f;

            // setting state varible of previous build order step
            CurrentBuildOrderStep.PrevBuildOrderStep.IsActive = false;
            CurrentBuildOrderStep.PrevBuildOrderStep.IsDone = true;

            // setting state varible of current build order step
            CurrentBuildOrderStep.IsActive = true;
            CurrentBuildOrderStep.IsDone = false;

            // if next build order step is null we are finish
            if (CurrentBuildOrderStep.NextBuildOrderStep == null)
                Global.BoState = BoState.Finish;

            StepRefresh(this, new EventArgs());
        }

        public void PrevStep()
        {
            if (CurrentBuildOrderStep.PrevBuildOrderStep == null)
                return;

            // set the next build order step
            CurrentBuildOrderStep = CurrentBuildOrderStep.PrevBuildOrderStep;

            CurrentBuildOrderStep.NextBuildOrderStep.TimeActive = 0f;

            // setting state varible of previous build order step
            CurrentBuildOrderStep.NextBuildOrderStep.IsActive = false;
            CurrentBuildOrderStep.NextBuildOrderStep.IsDone = false;

            // setting state varible of current build order step
            CurrentBuildOrderStep.IsActive = true;
            CurrentBuildOrderStep.IsDone = false;

            StepRefresh(this, new EventArgs());
        }

        private void ResetBuildOrderSteps()
        {
            BuildOrderStep bos = FirstBuildOrderStep;
            do
            {
                bos.IsDone = false;
                bos.IsActive = false;
                bos.TimeActive = 0f;
                bos = bos.NextBuildOrderStep;
            } while (bos != null);

            CurrentBuildOrderStep = FirstBuildOrderStep;
            FirstBuildOrderStep.IsActive = true;
        }

        private void ParseBuildOrderString(string buildOrderString)
        {
            try
            {
                BuildOrderStep lastBuildOrderStep = null;
                string comment = "";

                string[] lines = buildOrderString.Split('\n');
                foreach (string lineRaw in lines)
                {
                    BuildOrderStep bos = new BuildOrderStep();
                    string line = lineRaw.Trim();

                    // if line is a comment or empty skip this loop
                    if (line.Length == 0 || line[0] == '#')
                    {
                        if (comment.Length > 0)
                            comment += "\n";
                        comment += line;
                        continue;
                    }

                    int seperatorPos = line.IndexOf(':');

                    // parse requirements
                    string[] requirements = line.Substring(0, seperatorPos).Replace(" ", "").Split(',');
                    foreach (string requirement in requirements)
                    {
                        switch (GetRequirementSuffix(requirement))
                        {
                            case "S":
                                bos.SupplyRequired = GetRequirementPrefix(requirement);
                                break;
                            case "SC":
                                bos.SupplyCapRequired = GetRequirementPrefix(requirement);
                                break;
                            case "F":
                                bos.FoodRequired = GetRequirementPrefix(requirement);
                                break;
                            case "W":
                                bos.WoodRequired = GetRequirementPrefix(requirement);
                                break;
                            case "G":
                                bos.GoldRequired = GetRequirementPrefix(requirement);
                                break;
                            case "ST":
                                bos.StoneRequired = GetRequirementPrefix(requirement);
                                break;
                            case "T":
                                bos.TimeRequired = GetRequirementPrefix(requirement);
                                break;
                        }
                    }

                    // parse instructions
                    string instruction = line.Substring(seperatorPos + 1).Trim();

                    // if lastBuildOrderStep is null it has to be the first
                    // build order step
                    if (lastBuildOrderStep == null)
                    {
                        bos.IsActive = true;
                        bos.Instructions.Add(instruction);
                        lastBuildOrderStep = bos;
                        FirstBuildOrderStep = bos;

                        bos.Comment = comment;
                        comment = "";
                    }
                    else
                    {
                        // create new buildOrderStep if requirements not equal
                        // otherwise just add instruction to last buildOrderStep
                        if (bos.RequirementsEqual(lastBuildOrderStep))
                        {
                            lastBuildOrderStep.Instructions.Add(instruction);
                        }
                        else
                        {
                            bos.Instructions.Add(instruction);

                            lastBuildOrderStep.NextBuildOrderStep = bos;
                            bos.PrevBuildOrderStep = lastBuildOrderStep;

                            lastBuildOrderStep = bos;

                            bos.Comment = comment;
                            comment = "";
                        }

                    }
                }

                CurrentBuildOrderStep = FirstBuildOrderStep;
            }
            catch (Exception)
            {
                FirstBuildOrderStep = null;
                CurrentBuildOrderStep = null;
                throw new Exception("parsing error");
            }
        }

        private int GetRequirementPrefix(string requirement)
        {
            return int.Parse(Regex.Match(requirement, @"\d+").Value);
        }

        private string GetRequirementSuffix(string requirement)
        {
            return new String(requirement.Where(Char.IsLetter).ToArray()).ToUpper();
        }
    }
}
