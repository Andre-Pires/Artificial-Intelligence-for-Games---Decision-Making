using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Action> DomainActions { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionSequence { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth { get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> domainActions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.DomainActions = domainActions;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionSequence = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.PositiveInfinity;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;
            var startTime = Time.realtimeSinceStartup;

            BestAction = null;

            while (CurrentDepth >= 0 && TotalActionCombinationsProcessed <= ActionCombinationsProcessedPerFrame)
            {
                processedActions++;
                float currentValue = Models[CurrentDepth].CalculateDiscontentment(Goals);

                if (CurrentDepth >= MAX_DEPTH)
                {
                    if (currentValue < BestDiscontentmentValue)
                    {
                        this.BestDiscontentmentValue = currentValue;
                        BestAction = ActionSequence[0];
                        this.ActionSequence.CopyTo(BestActionSequence, 0);
                    }

                    CurrentDepth--;
                    continue;
                }

                Action nextAction = Models[CurrentDepth].GetNextAction();

                if (nextAction != null)
                {
                    Models[CurrentDepth + 1] = Models[CurrentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(Models[CurrentDepth + 1]);
                    ActionSequence[CurrentDepth] = nextAction;
                    CurrentDepth++;
                }
                else
                {
                    CurrentDepth--;
                }
            }

            this.TotalActionCombinationsProcessed = processedActions;
            this.TotalProcessingTime = Time.realtimeSinceStartup - startTime;
            return BestAction;
        }
    }
}