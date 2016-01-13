using System;
using Microsoft.SPOT;
using StaMa;

namespace SampleSimpleStateMachineNETMF
{
    #region DevelopersGuide_classStateConfigurationUtils
    public static class StateConfigurationUtils
    {
        public static string Format(StateConfiguration stateConfiguration)
        {
            StateConfigurationWriter writer = new StateConfigurationWriter();
            stateConfiguration.PassThrough(writer);
            return writer.ToString();
        }
        
        private class StateConfigurationWriter : IStateConfigurationVisitor
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder(256);

            public override string ToString()
            {
                return text.ToString();
            }

            public void State(State state)
            {
                text.Append(state.Name);
            }

            public void StateAny()
            {
                text.Append("*");
            }

            public void BeginSubStates()
            {
                text.Append("(");
            }

            public void EndSubStates()
            {
                text.Append(")");
            }

            public void NextSubState()
            {
                text.Append(",");
            }
        }
    }
    #endregion
}
