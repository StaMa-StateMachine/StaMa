using System;
using System.Reflection;
using Microsoft.SPOT;
using MFUnitTest.Framework;

namespace StaMaTestNETMF
{
    public class Program
    {
        public static void Main()
        {
            Assembly testAssembly = Assembly.GetAssembly(typeof(Program));
            int failedTests = 0;
            int passedTests = 0;

            foreach (Type testClass in testAssembly.GetTypes())
            {
                int index = testClass.Name.LastIndexOf("Tests");
                if (index >= 0 && index == testClass.Name.Length - 5)
                {
                    MethodInfo[] testMethods = testClass.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    object testClassInstance = testClass.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    foreach (MethodInfo testMethod in testMethods)
                    {
                        if (testMethod.ReturnType != typeof(void))
                        {
                            continue;
                        }

                        //if (testClass.FullName != "StaMaTest.SaveStateResumeTests")
                        //{
                        //    if (testMethod.Name != "SaveStateResume_HappyPathNoExecuteEntryActions_BehavesAsExpected")
                        //    {
                        //        continue;
                        //    }
                        //}

                        try
                        {
                            testMethod.Invoke(testClassInstance, new object[] { });
                            Debug.Print("PASSED " + testClass.FullName + "." + testMethod.Name);
                            passedTests += 1;
                        }
                        catch (AssertionException ex)
                        {
                            failedTests += 1;
                            Debug.Print("FAILED " + testClass.FullName + "." + testMethod.Name + ": " + ex.Message);
                        } 
                        catch (Exception ex)
                        {
                            failedTests += 1;
                            Debug.Print("FAILED " + testClass.FullName + "." + testMethod.Name + " - Threw Exception:" + ex.Message);
                        }
                    }
                }
            }

            Debug.Print("Executed " + (passedTests + failedTests).ToString() + " tests:");
            if (failedTests == 0)
            {
                Debug.Print("All tests PASSED.");
            }
            else
            {
                Debug.Print("FAILED " + failedTests.ToString() + ", PASSED " + passedTests.ToString());
            }
        }
    }
}


namespace System
{
    static class Console
    {
        public static void WriteLine(String format, params object[] args)
        {
        }
    }
}
