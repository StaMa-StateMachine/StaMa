using System;
using System.Collections;
using StaMa;

#if !MF_FRAMEWORK
using NUnit.Framework;
using NUnit.Framework.Constraints;
#else
using MFUnitTest.Framework;
using MFUnitTest.Framework.Constraints;
using Microsoft.SPOT;
#endif


namespace StaMaTest
{
    class ActionRecorder
    {
        ArrayList executedActions = new ArrayList();

        public StaMa.StateMachineActionCallback CreateAction(string actionName)
        {
            return delegate(StateMachine s, object ev, EventArgs evArgs) { this.Add(actionName); };
        }

        public StaMa.StateMachineDoActionCallback CreateDoAction(string actionName)
        {
            return delegate(StateMachine s) { this.Add(actionName); };
        }

        public StaMa.StateMachine.TraceDispatchTriggerEventEventHandler CreateTraceMethod(string actionName)
        {
            return delegate(StateMachine s, object ev, EventArgs evArgs) { this.Add(actionName); };
        }

        public void Add(string actionName)
        {
            executedActions.Add(actionName);
        }

        public void Clear()
        {
            executedActions.Clear();
        }

        public String[] RecordedActions
        {
            get
            {
                return (String[])executedActions.ToArray(typeof(String));
            }
        }
    }


    class StateConfigurationIsMatching : IEqualityComparer
    {
        private static StateConfigurationIsMatching m_instance;

        private StateConfigurationIsMatching()
        {
        }

        static StateConfigurationIsMatching()
        {
            m_instance = new StateConfigurationIsMatching();
        }

        public static IEqualityComparer Instance
        {
            get
            {
                return m_instance;
            }
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            StateConfiguration stateConfigurationLhs = x as StateConfiguration;
            if (stateConfigurationLhs == null)
            {
                throw new ArgumentNullException("x");
            }
            StateConfiguration stateConfigurationRhs = y as StateConfiguration;
            if (stateConfigurationRhs == null)
            {
                throw new ArgumentNullException("y");
            }
            bool isMatching = stateConfigurationLhs.IsMatching(stateConfigurationRhs);
            return isMatching;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
