using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace MyRealDWG
{
    class MyDebugger
    {
        public delegate void OnMessageHandler(string msg);
        static public event OnMessageHandler OnMessage;

        static public Hashtable mOverrides;

        /// <summary>
        /// This initializes the hashtable with all the functions and properties which are overridden in the class.
        /// It is using Reflection to achieve this.
        /// </summary>
        static public void initHashTable()
        {
            mOverrides = new Hashtable();

            Type t = typeof(MyHost);
            MethodInfo[] methods = t.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (!mOverrides.Contains(method.Name) && method.DeclaringType == t)
                    mOverrides.Add(method.Name, true);
            }
        }

        /// <summary>
        /// Checks if the user chose to use the overriden version of the given function or property 
        /// </summary>
        /// <param name="func">name of the function to be checked</param>
        /// <param name="input">the input parameter value of the function to be checked</param>
        public static bool isOverriden(
            string func, 
            string input)
        {
            OnMessage("Function " + func + " called with parameter " + input);

            if (!mOverrides.Contains(func))
            {
                MessageBox.Show("Function " + func + " is not added to the list");
                return false;
            }

            return (bool)mOverrides[func];
        }

        /// <summary>
        /// This simply raises an event with the string value passed in, so that the subscribers will be notified about it.
        /// </summary>        
        /// <param name="value">return value of the function</param>
        public static string logReturn(
            string value)
        {
            OnMessage("... returns value " + value);

            return value;
        }

        /// <summary>
        /// This simply raises an event with the string value passed in, so that the subscribers will be notified about it.
        /// This is called if we do not modify the behaviour of the function or property, just log the return value.
        /// </summary>        
        /// <param name="func">name of the function</param>
        /// <param name="value">return value of the function</param>
        public static string logReturn(
            string func, 
            string value)
        {
            OnMessage(func + " returns value " + value);

            return value;
        }
    }
}
