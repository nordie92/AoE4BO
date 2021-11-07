using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AoE4BO
{
    public class BuildOrder
    {
        public BuildOrderStep FirstBuildOrderStep { get; set; }
        public BuildOrderStep CurrentBuildOrderStep { get; set; }
        private Timer _timerUpdate;
        private Stopwatch _stopwatch;

        public BuildOrder(string buildOrderString)
        {
            Global.BoState = BoState.Idle;
            ParseBuildOrderString(buildOrderString);
        }

        public void Start()
        {
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
                CurrentBuildOrderStep = CurrentBuildOrderStep.NextBuildOrderStep;

                CurrentBuildOrderStep.PrevBuildOrderStep.IsActive = false;
                CurrentBuildOrderStep.PrevBuildOrderStep.IsDone = true;

                CurrentBuildOrderStep.IsActive = true;
                CurrentBuildOrderStep.IsDone = false;

                if (CurrentBuildOrderStep.NextBuildOrderStep != null)
                {
                    CurrentBuildOrderStep.NextBuildOrderStep.IsActive = false;
                    CurrentBuildOrderStep.NextBuildOrderStep.IsDone = false;
                }
                else
                {
                    Global.BoState = BoState.Finish;
                }

                if (Global.Settings.PlaySound)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"pfeifen.wav");
                    player.Play();
                }
            }

            // add time
            if (CurrentBuildOrderStep.NextBuildOrderStep != null)
            {
                CurrentBuildOrderStep.NextBuildOrderStep.TimeActive += deltaTime;
            }
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

                string[] lines = buildOrderString.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Length == 0)
                        continue;

                    BuildOrderStep bos = new BuildOrderStep();

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
                    string instruction = line.Substring(seperatorPos + 1);

                    // set lastBOS if it null
                    if (lastBuildOrderStep == null)
                    {
                        bos.IsActive = true;
                        bos.Instructions.Add(instruction);
                        lastBuildOrderStep = bos;
                        FirstBuildOrderStep = bos;
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
