#region IStateMachineTemplateVisitor.cs file
//
// StaMa state machine controller library, https://github.com/StaMa-StateMachine/StaMa
//
// Copyright (c) 2013-2016, Roland Schneider. All rights reserved.
//
#endregion


using System;

namespace StaMa
{
    internal static class FrameworkAbstractionUtils
    {
        public static string StringFormat(string fmt, params object[] args)
        {
#if !MF_FRAMEWORK
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, fmt, args);
#else
            // TODO: Find a better implementation.
            System.Text.StringBuilder sb = new System.Text.StringBuilder(fmt);
            for (int i = 0; i < args.Length; i++)
            {
                string placeholder = String.Concat("{", i.ToString(), "}");
                string arg = args[i].ToString();
                sb = sb.Replace(placeholder, arg);
            }
            return sb.ToString();
#endif
        }


        public static string Int32ToString(Int32 value)
        {
#if !MF_FRAMEWORK
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
#else
            return value.ToString();
#endif
        }


        public static bool StringIsNullOrEmpty(String value)
        {
#if !MF_FRAMEWORK
            return String.IsNullOrEmpty(value);
#else
            if (value != null)
            {
                return (value.Length == 0);
            }
            return true;
#endif
        }
    }
}
