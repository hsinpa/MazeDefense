using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UtilityBase
{
    public class UtilityTheory 
    {
        public delegate void RequestCallback(string action);

        public async Task Request(UtilityCard utilityCard, RequestCallback requestCallback) {
           string actionName = await FindActionByScore(utilityCard);

            if (requestCallback != null)
                requestCallback(actionName);
        }

        private async Task<string> FindActionByScore(UtilityCard utilityCard) {
            float score = 0;
            string actionName = "";

            if (utilityCard.requestCard != null) {
                int length = utilityCard.requestCard.Length;

                for (int i = 0; i < length; i++) {

                    if (utilityCard.requestCard[i].Execute != null) {
                        float utilityScore = utilityCard.requestCard[i].Execute();

                        if (utilityScore > score) {
                            score = utilityScore;
                            actionName = utilityCard.requestCard[i].action;
                        }
                    }
                }
            }
           
            return actionName;
        }

        private string FindActionByWeight(UtilityCard utilityCard, int topK)
        {
            return "";
        }


        public struct UtilityCard {
            public UtilityRequest[] requestCard;
        }

        public struct UtilityRequest {
            public string action;

            public System.Func<float> Execute;
        }

        public struct UtilityResult
        {
            public string action;
        }


    }
}