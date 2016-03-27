// ****************************************************************
// Major parts of this software are cut out from NUnit.Framework.dll (http://nunit.org, https://github.com/nunit) and code has been altered or adapted to fit testing purposes on NETMF.
// This source code is *not* original NUnit code.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;
using System.Text;
using MFUnitTest.Framework.Constraints;


namespace MFUnitTest.Framework
{
    public class MFUnitTestSelfTests_Explicit
    {
        public void Test()
        {
            string messageText = "wuiertu";
            Object object1 = new Object();
            Object object2 = new Object();
            Object[] collection1 = new String[] { "s1", "s2" };
            String[] collection2 = new String[] { "s1", "s2" };
            String[] collection3 = new String[] { "s1" };
            String[] collection4 = new String[] { "s1", "s3" };
            String[] collection5 = new String[] { "s2", "s1" };
            Object[] collection6 = new Object[] { "s1", 4 };
            Object[] collection7 = new Object[] { "s1", "4" };

            try
            {
                Assert.That(delegate() { }, Throws.Nothing);
                Assert.That(delegate() { throw new ArgumentException("message"); }, Throws.TypeOf(typeof(ArgumentException)));
                Assert.That(delegate() { throw new ArgumentException("message"); }, Throws.TypeOf(typeof(ArgumentException)).With.Message.EqualTo("message"));
                Assert.That(delegate() { throw new ArgumentException("message asdf text"); }, Throws.TypeOf(typeof(ArgumentException)).With.Message.Contains("asdf"));
                Assert.That(delegate() { throw new ArgumentException("message AsdF text"); }, Throws.TypeOf(typeof(ArgumentException)).With.Message.Contains("asdf").IgnoreCase);

                Assert.That(delegate() { Assert.That(delegate() { throw new Exception("Exception of type 'System.Exception' was thrown."); }, Throws.Nothing, messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.StartsWith(messageText + "\r\n  Expected: No Exception to be thrown\r\n  But was:   (Exception of type 'System.Exception' was thrown.)"));

                Assert.That(delegate() { Assert.That(delegate() { }, Throws.TypeOf(typeof(ArgumentException)), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: <System.ArgumentException>\r\n  But was:  no exception thrown\r\n"));
                Assert.That(delegate() { throw new ArgumentException("other"); }, Throws.TypeOf(typeof(ArgumentException)).With.Message.Not.EqualTo("message"));

                Assert.That(delegate() { Assert.That(true, Is.EqualTo(true)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(false, Is.EqualTo(false)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(false, Is.EqualTo(true), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: True\r\n  But was:  False\r\n"));
                Assert.That(delegate() { Assert.That(true, Is.EqualTo(false), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: False\r\n  But was:  True\r\n"));

                Assert.That(delegate() { Assert.That(0, Is.EqualTo(0)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(5, Is.EqualTo(5)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(5, Is.EqualTo(0), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: 0\r\n  But was:  5\r\n"));
                Assert.That(delegate() { Assert.That(5, Is.Not.EqualTo(0), messageText); }, Throws.Nothing);

                Assert.That(delegate() { Assert.That("Hello", Is.EqualTo("Hello")); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That("", Is.EqualTo("")); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That("Hello", Is.EqualTo(""), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: <string.Empty>\r\n  But was:  \"Hello\"\r\n"));
                Assert.That(delegate() { Assert.That("Hello", Is.Not.EqualTo(""), messageText); }, Throws.Nothing);

                Assert.That(delegate() { Assert.That(null, Is.EqualTo(null)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(object1, Is.Not.EqualTo(null)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(object1, Is.EqualTo(object1)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(object1, Is.EqualTo(object2), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: <System.Object>\r\n  But was:  <System.Object>\r\n"));
                Assert.That(delegate() { Assert.That(object1, Is.EqualTo(null), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: null\r\n  But was:  <System.Object>\r\n"));
                Assert.That(delegate() { Assert.That(null, Is.EqualTo(new Object()), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: <System.Object>\r\n  But was:  null\r\n"));

                Assert.That(delegate() { Assert.That(collection1, Is.EqualTo(collection1)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(collection1, Is.EqualTo(collection2)); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(collection1, Is.EqualTo(collection3), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected is <System.String[1]>, actual is <System.String[2]>\r\n  Values differ at index [1]\r\n"));
                Assert.That(delegate() { Assert.That(collection1, Is.EqualTo(collection4), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected and actual are both <System.String[2]>\r\n  Values differ at index [1]\r\n  Expected: \"s3\"\r\n  But was:  \"s2\"\r\n"));
                Assert.That(delegate() { Assert.That(collection1, Is.EqualTo(collection5), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected and actual are both <System.String[2]>\r\n  Values differ at index [0]\r\n  Expected: \"s2\"\r\n  But was:  \"s1\"\r\n"));
                Assert.That(delegate() { Assert.That(collection1, Is.EqualTo(collection6), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected is <System.Object[2]>, actual is <System.String[2]>\r\n  Values differ at index [1]\r\n  Expected: 4\r\n  But was:  \"s2\"\r\n"));
                Assert.That(delegate() { Assert.That(null, Is.EqualTo(collection1), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: <System.String[]>\r\n  But was:  null\r\n"));

                Assert.That(delegate() { Assert.That(5, Is.EqualTo("5").Using(NumberEqualityComparer.Instance), messageText); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(3, Is.EqualTo("5").Using(NumberEqualityComparer.Instance), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: \"5\"\r\n  But was:  3\r\n"));
                Assert.That(delegate() { Assert.That(collection6, Is.EqualTo(collection7), messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected and actual are both <System.Object[2]>\r\n  Values differ at index [1]\r\n  Expected: \"4\"\r\n  But was:  4\r\n"));
                Assert.That(delegate() { Assert.That(collection6, Is.EqualTo(collection7).Using(NumberEqualityComparer.Instance), messageText); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(5, Is.EqualTo("5").Using(null), messageText); }, Throws.TypeOf(typeof(ArgumentNullException)));

                Assert.That(delegate() { Assert.That(null, Is.Null); }, Throws.Nothing);
                Assert.That(delegate() { Assert.That(new Object(), Is.Null, messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: null\r\n  But was:  <System.Object>\r\n"));
                Assert.That(delegate() { Assert.That(null, Is.Not.Null, messageText); }, Throws.TypeOf(typeof(AssertionException)).With.Message.EqualTo(messageText + "\r\n  Expected: not null\r\n  But was:  null\r\n"));
                Assert.That(delegate() { Assert.That(new Object(), Is.Not.Null); }, Throws.Nothing);
            }
            catch (AssertionException ex)
            {
                Microsoft.SPOT.Debug.Print(ex.Message + "\r\n");
                Microsoft.SPOT.Debug.Print(ex.StackTrace + "\r\n");
                throw;
            }
        }
    }


    class NumberEqualityComparer : IEqualityComparer
    {
        private static NumberEqualityComparer m_instance;

        private NumberEqualityComparer()
        {
        }

        static NumberEqualityComparer()
        {
            m_instance = new NumberEqualityComparer();
        }

        public static IEqualityComparer Instance
        {
            get
            {
                return m_instance;
            }
        }

        bool IEqualityComparer.Equals(Object x, Object y)
        {
            return (Object.Equals(ConvertToString(x), ConvertToString(y)));
        }


        private static String ConvertToString(Object value)
        {
            if (!StringEx.IsString(value))
            {
                return value.ToString();
            }
            else
            {
                return (String)value;
            }
        }

        int IEqualityComparer.GetHashCode(Object obj)
        {
            throw new NotImplementedException();
        }
    }


    public delegate void TestDelegate();


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TestFixtureAttribute : Attribute
    {
        public TestFixtureAttribute()
        {
        }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public class TestAttribute : Attribute
    {
        public TestAttribute()
        {
        }
    }


    public static class Assert
    {
        public static void That(object actual, IResolveConstraint constraint)
        {
            That(actual, constraint, null);
        }

        public static void That(TestDelegate code, IResolveConstraint expression)
        {
            That((object)code, expression, null);
        }

        public static void That(TestDelegate code, IResolveConstraint expression, string message)
        {
            That((object)code, expression, message);
        }

        public static void That(object actual, IResolveConstraint expression, string message)
        {
            Constraint constraint = expression.Resolve();
            if (!constraint.Matches(actual))
            {
                MessageWriter writer = new MessageWriter(message);
                constraint.WriteMessageTo(writer);
                throw new AssertionException(writer.ToString());
            }
        }
    }


    public class Is
    {
        public static EqualConstraint EqualTo(object expected)
        {
            return new EqualConstraint(expected);
        }

        public static ConstraintExpression Not
        {
            get
            {
                return new ConstraintExpression().Not;
            }
        }

        public static NullConstraint Null
        {
            get
            {
                return new NullConstraint();
            }
        }
    }


    public class Throws
    {
        public static InstanceOfTypeConstraint InstanceOf(Type expectedType)
        {
            return Exception.InstanceOf(expectedType);
        }

        public static ExactTypeConstraint TypeOf(Type expectedType)
        {
            return Exception.TypeOf(expectedType);
        }

        public static ResolvableConstraintExpression Exception
        {
            get
            {
                return new ConstraintExpression().Append((SelfResolvingOperator)new ThrowsOperator());
            }
        }

        public static ThrowsNothingConstraint Nothing
        {
            get
            {
                return new ThrowsNothingConstraint();
            }
        }
    }



    public class AssertionException : Exception
    {
        public AssertionException(string message)
            : base(message)
        {
        }
    }


    public class MessageWriter : StringWriter
    {
        public MessageWriter(string message)
            : base(new StringBuilder())
        {
            if (message != null)
            {
                this.WriteLine(message);
            }
        }

        public void DisplayDifferences(Constraint constraint)
        {
            this.WriteExpectedLine(constraint);
            this.WriteActualLine(constraint);
        }

        public void DisplayDifferences(object expected, object actual)
        {
            this.WriteExpectedLine(expected);
            this.WriteActualLine(actual);
        }

        //public abstract void DisplayDifferences(object expected, object actual);
        ////public abstract void DisplayDifferences(object expected, object actual);
        //public abstract void DisplayStringDifferences(string expected, string actual, int mismatch, bool ignoreCase, bool clipping);
        public void WriteActualValue(object actual)
        {
            this.WriteValue(actual);
        }

        //public abstract void WriteCollectionElements(IEnumerable collection, int start, int max);

        public void WriteConnector(string connector)
        {
            this.Write(" " + connector + " ");
        }

        private void WriteExpectedLine(object expected)
        {
            this.Write("  Expected: ");
            this.WriteExpectedValue(expected);
            this.WriteLine();
        }


        private void WriteActualLine(object actual)
        {
            this.Write("  But was:  ");
            this.WriteActualValue(actual);
            this.WriteLine();
        }


        public void WriteExpectedValue(object expected)
        {
            this.WriteValue(expected);
        }

        //public void WriteMessageLine(string message, params object[] args)
        //{
        //    this.WriteMessageLine(0, message, args);
        //}

        //public abstract void WriteMessageLine(int level, string message, params object[] args);
        //public abstract void WriteModifier(string modifier);
        public void WritePredicate(string predicate)
        {
            this.Write(predicate + " ");
        }


        public void WriteValue(object val)
        {
            if (val == null)
            {
                this.Write("null");
            }
            //else if (val.GetType().IsArray)
            //{
            //    this.WriteArray((Array)val);
            //}
            else if (StringEx.IsString(val))
            {
                this.WriteString((string)val);
            }
            //else if (val is IEnumerable)
            //{
            //    this.WriteCollectionElements((IEnumerable)val, 0, 10);
            //}
            //else if (val is char)
            //{
            //    this.WriteChar((char)val);
            //}
            //else if (val is double)
            //{
            //    this.WriteDouble((double)val);
            //}
            //else if (val is float)
            //{
            //    this.WriteFloat((float)val);
            //}
            //else if (val is decimal)
            //{
            //    this.WriteDecimal((decimal)val);
            //}
            //else if (val is DateTime)
            //{
            //    this.WriteDateTime((DateTime)val);
            //}
            else if (val.GetType().IsValueType)
            {
                this.Write(val.ToString());
            }
            else
            {
                this.Write("<" + val.ToString() + ">");
            }
        }


        public void WriteMessageLine(int level, string message, params object[] args)
        {
            if (message != null)
            {
                while (level-- >= 0)
                {
                    this.Write("  ");
                }
                if ((args != null) && (args.Length > 0))
                {
                    message = StringEx.Format(message, args);
                }
                this.WriteLine(message);
            }
        }

        
        private void WriteString(string s)
        {
            if (s == string.Empty)
            {
                this.Write("<string.Empty>");
            }
            else
            {
                this.Write("\"" + s + "\"");
            }
        }

        private void WriteExpectedLine(Constraint constraint)
        {
            this.Write("  Expected: ");
            constraint.WriteDescriptionTo(this);
            this.WriteLine();
        }


        private void WriteActualLine(Constraint constraint)
        {
            this.Write("  But was:  ");
            constraint.WriteActualValueTo(this);
            this.WriteLine();
        }

        public void WriteModifier(string modifier)
        {
            this.Write(", " + modifier);
        }

        //public abstract int MaxLineLength { get; set; }
    }


    public class StringWriter : System.IO.TextWriter
    {
        private StringBuilder m_sb;

        public StringWriter(StringBuilder sb)
        {
            if (sb == null)
            {
                throw new ArgumentNullException();
            }
            m_sb = sb;
        }

        public override string ToString()
        {
            return m_sb.ToString();
        }

        public override void Write(string value)
        {
            m_sb.Append(value);
        }

        public override void Write(char value)
        {
            m_sb.Append(value + "\r\n");
        }

        public override void Write(char[] buffer, int index, int count)
        {
            m_sb.Append(buffer, index, count);
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }


    internal static class StringEx
    {
        public static string Format(string fmt, params object[] args)
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

        public static bool IsString(object obj)
        {
            return (obj != null) ? (obj.GetType() == typeof(String)) : false;
        }
    }
}


namespace MFUnitTest.Framework.Constraints
{
    public interface IResolveConstraint
    {
        Constraint Resolve();
    }

    public abstract class Constraint : IResolveConstraint
    {
        protected object actual;
        private readonly object arg1;
        private readonly object arg2;
        private readonly int argcnt;
        private ConstraintBuilder builder;
        private string displayName;
        protected static object UNSET = new UnsetObject();

        protected Constraint()
        {
            this.actual = UNSET;
            this.argcnt = 0;
        }

        protected Constraint(object arg)
        {
            this.actual = UNSET;
            this.argcnt = 1;
            this.arg1 = arg;
        }

        protected Constraint(object arg1, object arg2)
        {
            this.actual = UNSET;
            this.argcnt = 2;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }

        public abstract bool Matches(object actual);


        Constraint IResolveConstraint.Resolve()
        {
            return ((this.builder == null) ? this : this.builder.Resolve());
        }


        public virtual void WriteMessageTo(MessageWriter writer)
        {
            writer.DisplayDifferences(this);
        }


        public virtual void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteActualValue(this.actual);
        }


        public abstract void WriteDescriptionTo(MessageWriter writer);


        internal void SetBuilder(ConstraintBuilder builder)
        {
            this.builder = builder;
        }


        protected string DisplayName
        {
            get
            {
                //if (this.displayName == null)
                //{
                //    this.displayName = base.GetType().Name.ToLower();
                //    if (this.displayName.EndsWith("`1") || this.displayName.EndsWith("`2"))
                //    {
                //        this.displayName = this.displayName.Substring(0, this.displayName.Length - 2);
                //    }
                //    if (this.displayName.EndsWith("constraint"))
                //    {
                //        this.displayName = this.displayName.Substring(0, this.displayName.Length - 10);
                //    }
                //}
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }


        protected virtual string GetStringRepresentation()
        {
            switch (this.argcnt)
            {
                default:
                    return "<" + this.DisplayName + ">";
                case 1:
                    return "<" + this.DisplayName + " " + this.arg1.ToString() + ">";
                case 2:
                    return "<" + this.DisplayName + " " + this.arg1.ToString() + " " + this.arg2.ToString() + ">";
            }
        }


        public ConstraintExpression And
        {
            get
            {
                ConstraintBuilder builder = this.builder;
                if (builder == null)
                {
                    builder = new ConstraintBuilder();
                    builder.Append(this);
                }
                builder.Append(new AndOperator());
                return new ConstraintExpression(builder);
            }
        }
 

        public ConstraintExpression With
        {
            get
            {
                return this.And;
            }
        }
 

        private class UnsetObject
        {
            public override string ToString()
            {
                return "UNSET";
            }
        }
    }


    public class ConstraintBuilder
    {
        private readonly ConstraintStack constraints;
        private object lastPushed;
        private readonly OperatorStack ops;

        public ConstraintBuilder()
        {
            this.ops = new OperatorStack(this);
            this.constraints = new ConstraintStack(this);
        }

        public void Append(Constraint constraint)
        {
            if (this.lastPushed is ConstraintOperator)
            {
                this.SetTopOperatorRightContext(constraint);
            }
            this.constraints.Push(constraint);
            this.lastPushed = constraint;
            constraint.SetBuilder(this);
        }

        public void Append(ConstraintOperator op)
        {
            op.LeftContext = this.lastPushed;
            if (this.lastPushed is ConstraintOperator)
            {
                this.SetTopOperatorRightContext(op);
            }
            this.ReduceOperatorStack(op.LeftPrecedence);
            this.ops.Push(op);
            this.lastPushed = op;
        }

        private void ReduceOperatorStack(int targetPrecedence)
        {
            while (!this.ops.Empty && (this.ops.Top.RightPrecedence < targetPrecedence))
            {
                this.ops.Pop().Reduce(this.constraints);
            }
        }

        public Constraint Resolve()
        {
            if (!this.IsResolvable)
            {
                throw new InvalidOperationException("A partial expression may not be resolved");
            }
            while (!this.ops.Empty)
            {
                this.ops.Pop().Reduce(this.constraints);
            }
            return this.constraints.Pop();
        }

        private void SetTopOperatorRightContext(object rightContext)
        {
            int leftPrecedence = this.ops.Top.LeftPrecedence;
            this.ops.Top.RightContext = rightContext;
            if (this.ops.Top.LeftPrecedence > leftPrecedence)
            {
                ConstraintOperator op = this.ops.Pop();
                this.ReduceOperatorStack(op.LeftPrecedence);
                this.ops.Push(op);
            }
        }

        public bool IsResolvable
        {
            get
            {
                // return ((this.lastPushed is Constraint) || (this.lastPushed is SelfResolvingOperator));
                return (this.lastPushed is Constraint);
            }
        }

        public class ConstraintStack
        {
            private ConstraintBuilder builder;
            private Stack stack = new Stack();  // of Constraint
             
            public ConstraintStack(ConstraintBuilder builder)
            {
                this.builder = builder;
            }

            public Constraint Pop()
            {
                Constraint constraint = (Constraint)this.stack.Pop();
                constraint.SetBuilder(null);
                return constraint;
            }

            public void Push(Constraint constraint)
            {
                this.stack.Push(constraint);
                constraint.SetBuilder(this.builder);
            }

            public bool Empty
            {
                get
                {
                    return (this.stack.Count == 0);
                }
            }

            public Constraint Top
            {
                get
                {
                    return (Constraint)this.stack.Peek();
                }
            }
        }

        public class OperatorStack
        {
            private Stack stack = new Stack(); // of ConstraintOperator

            public OperatorStack(ConstraintBuilder builder)
            {
            }

            public ConstraintOperator Pop()
            {
                return (ConstraintOperator)this.stack.Pop();
            }

            public void Push(ConstraintOperator op)
            {
                this.stack.Push(op);
            }

            public bool Empty
            {
                get
                {
                    return (this.stack.Count == 0);
                }
            }

            public ConstraintOperator Top
            {
                get
                {
                    return (ConstraintOperator)this.stack.Peek();
                }
            }
        }
    }



    public class EqualConstraint : Constraint
    {
        private readonly object expected;
        private FailurePointList failurePoints;
        private IEqualityComparer comparer;

        //private static readonly string StringsDiffer_1;
        //private static readonly string StringsDiffer_2;
        //private static readonly string StreamsDiffer_1;
        //private static readonly string StreamsDiffer_2;
        private static readonly string CollectionType_1;
        private static readonly string CollectionType_2;
        private static readonly string ValuesDiffer_1;
        //private static readonly string ValuesDiffer_2;
 

        static EqualConstraint()
        {
            //StringsDiffer_1 = "String lengths are both {0}. Strings differ at index {1}.";
            //StringsDiffer_2 = "Expected string length {0} but was {1}. Strings differ at index {2}.";
            //StreamsDiffer_1 = "Stream lengths are both {0}. Streams differ at offset {1}.";
            //StreamsDiffer_2 = "Expected Stream length {0} but was {1}.";
            CollectionType_1 = "Expected and actual are both {0}";
            CollectionType_2 = "Expected is {0}, actual is {1}";
            ValuesDiffer_1 = "Values differ at index {0}";
            //ValuesDiffer_2 = "Values differ at expected index {0}, actual index {1}";
        }


        public EqualConstraint(object expected)
            : base(expected)
        {
            this.expected = expected;
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            this.failurePoints = new FailurePointList();
            return ObjectsEqual(this.expected, actual);
        }


        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteExpectedValue(this.expected);
        }


        public override void WriteMessageTo(MessageWriter writer)
        {
            this.DisplayDifferences(writer, this.expected, base.actual, 0);
        }


        private void DisplayDifferences(MessageWriter writer, object expected, object actual, int depth)
        {
            //bool isoftype = typeof(String[]).IsInstanceOfType(expected);
            //bool isoftype2 = typeof(String).IsInstanceOfType(expected);
            //String expectedString = expected as String;
            //String actualString = actual as String;
            //if (expectedString != null && actualString != null)
            //if ((expected is string) && (actual is string))
            if (StringEx.IsString(expected) && StringEx.IsString(actual))
            {
                //this.DisplayStringDifferences(writer, (string)expected, (string)actual);
                writer.DisplayDifferences(expected, actual);
            }
            //else if ((expected is ICollection) && (actual is ICollection))
            //{
            //    this.DisplayCollectionDifferences(writer, (ICollection)expected, (ICollection)actual, depth);
            //}
            else if ((expected is IEnumerable) && (actual is IEnumerable))
            {
                this.DisplayEnumerableDifferences(writer, (IEnumerable)expected, (IEnumerable)actual, depth);
            }
            //else if ((expected is Stream) && (actual is Stream))
            //{
            //    this.DisplayStreamDifferences(writer, (Stream)expected, (Stream)actual, depth);
            //}
            //else if (this.tolerance != null)
            //{
            //    writer.DisplayDifferences(expected, actual, this.tolerance);
            //}
            else
            {
                writer.DisplayDifferences(expected, actual);
            }
        }


        private void DisplayEnumerableDifferences(MessageWriter writer, IEnumerable expected, IEnumerable actual, int depth)
        {
            this.DisplayTypesAndSizes(writer, expected, actual, depth);
            if (this.failurePoints.Count > depth)
            {
                FailurePoint failurePoint = (FailurePoint)this.failurePoints[depth];
                this.DisplayFailurePoint(writer, expected, actual, failurePoint, depth);
                if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
                {
                    this.DisplayDifferences(writer, failurePoint.ExpectedValue, failurePoint.ActualValue, ++depth);
                }
            }
        }


        private void DisplayFailurePoint(MessageWriter writer, IEnumerable expected, IEnumerable actual, FailurePoint failurePoint, int indent)
        {
            int[] arrayIndicesFromCollectionIndex = GetArrayIndicesFromCollectionIndex(expected, failurePoint.Position);
            writer.WriteMessageLine(indent, ValuesDiffer_1, new object[] { GetArrayIndicesAsString(arrayIndicesFromCollectionIndex) });
        }


        private static int[] GetArrayIndicesFromCollectionIndex(IEnumerable collection, int index)
        {
            int[] numArray = new int[1];
            numArray[0] = index;
            return numArray;
        }


        public static string GetArrayIndicesAsString(int[] indices)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            for (int i = 0; i < indices.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(indices[i].ToString());
            }
            builder.Append(']');
            return builder.ToString();
        }


        public static string GetTypeRepresentation(object obj)
        {
            Array array = obj as Array;
            if (array == null)
            {
                return StringEx.Format("<{0}>", obj.GetType());
            }
            StringBuilder builder = new StringBuilder();
            Type elementType = array.GetType();
            int num = 0;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType();
                num++;
            }
            builder.Append(elementType.ToString());
            builder.Append('[');
            builder.Append(array.Length);
            builder.Append(']');
            while (--num > 0)
            {
                builder.Append("[]");
            }
            return StringEx.Format("<{0}>", builder.ToString());
        }


        private void DisplayTypesAndSizes(MessageWriter writer, IEnumerable expected, IEnumerable actual, int indent)
        {
            string typeRepresentation = GetTypeRepresentation(expected);
            if (!(!(expected is ICollection) || (expected is Array)))
            {
                typeRepresentation = typeRepresentation + StringEx.Format(" with {0} elements", ((ICollection)expected).Count);
            }
            string str2 = GetTypeRepresentation(actual);
            if (!(!(actual is ICollection) || (actual is Array)))
            {
                str2 = str2 + StringEx.Format(" with {0} elements", ((ICollection)actual).Count);
            }
            if (typeRepresentation == str2)
            {
                writer.WriteMessageLine(indent, CollectionType_1, new object[] { typeRepresentation });
            }
            else
            {
                writer.WriteMessageLine(indent, CollectionType_2, new object[] { typeRepresentation, str2 });
            }
        }


        private static bool CanCompare(object x, object y)
        {
            if (((x is IEnumerable) && !StringEx.IsString(x)) || ((y is IEnumerable) && !StringEx.IsString(y)))
            {
                return false;
            }
            return true;
        }


        private bool ObjectsEqual(object expected, object actual)
        {
            if ((expected == null) && (actual == null))
            {
                return true;
            }
            if ((expected == null) || (actual == null))
            {
                return false;
            }
            if (object.ReferenceEquals(expected, actual))
            {
                return true;
            }
            //Type first = expected.GetType();
            //Type type = actual.GetType();
            //EqualityAdapter externalComparer = this.GetExternalComparer(expected, actual);
            if ((comparer != null)  &&  CanCompare(expected, actual))
            {
                return comparer.Equals(expected, actual);
            }
            //if (!((!first.IsArray || !type.IsArray) || this.compareAsCollection))
            //{
            //    return this.ArraysEqual((Array)expected, (Array)actual, ref tolerance);
            //}
            //if ((expected is IDictionary) && (actual is IDictionary))
            //{
            //    return this.DictionariesEqual((IDictionary)expected, (IDictionary)actual, ref tolerance);
            //}
            //if ((expected is DictionaryEntry) && (actual is DictionaryEntry))
            //{
            //    return this.DictionaryEntriesEqual((DictionaryEntry)expected, (DictionaryEntry)actual, ref tolerance);
            //}
            //if (((first.IsGenericType && (first.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
            //{
            //    Tolerance tolerance2 = new Tolerance(0);
            //    object obj2 = first.GetProperty("Key").GetValue(expected, null);
            //    object obj3 = type.GetProperty("Key").GetValue(actual, null);
            //    object obj4 = first.GetProperty("Value").GetValue(expected, null);
            //    object obj5 = type.GetProperty("Value").GetValue(actual, null);
            //    return (this.AreEqual(obj2, obj3, ref tolerance2) && this.AreEqual(obj4, obj5, ref tolerance));
            //}
            if (((expected is IEnumerable) && (actual is IEnumerable)) && (!StringEx.IsString(expected) || !StringEx.IsString(actual)))
            {
                return this.EnumerablesEqual((IEnumerable)expected, (IEnumerable)actual);
            }
            //if ((expected is string) && (actual is string))
            //{
            //    return this.StringsEqual((string)expected, (string)actual);
            //}
            //if ((expected is Stream) && (actual is Stream))
            //{
            //    return this.StreamsEqual((Stream)expected, (Stream)actual);
            //}
            //if ((expected is char) && (actual is char))
            //{
            //    return this.CharsEqual((char)expected, (char)actual);
            //}
            //if ((expected is DirectoryInfo) && (actual is DirectoryInfo))
            //{
            //    return DirectoriesEqual((DirectoryInfo)expected, (DirectoryInfo)actual);
            //}
            //if (Numerics.IsNumericType(expected) && Numerics.IsNumericType(actual))
            //{
            //    return Numerics.AreEqual(expected, actual, ref tolerance);
            //}
            //if ((tolerance != null) && (tolerance.Value is TimeSpan))
            //{
            //    TimeSpan span2;
            //    TimeSpan span = (TimeSpan)tolerance.Value;
            //    if ((expected is DateTime) && (actual is DateTime))
            //    {
            //        span2 = (TimeSpan)(((DateTime)expected) - ((DateTime)actual));
            //        return (span2.Duration() <= span);
            //    }
            //    if ((expected is DateTimeOffset) && (actual is DateTimeOffset))
            //    {
            //        span2 = (TimeSpan)(((DateTimeOffset)expected) - ((DateTimeOffset)actual));
            //        return (span2.Duration() <= span);
            //    }
            //    if ((expected is TimeSpan) && (actual is TimeSpan))
            //    {
            //        span2 = ((TimeSpan)expected) - ((TimeSpan)actual);
            //        return (span2.Duration() <= span);
            //    }
            //}
            //if (FirstImplementsIEquatableOfSecond(first, type))
            //{
            //    return InvokeFirstIEquatableEqualsSecond(expected, actual);
            //}
            //if (FirstImplementsIEquatableOfSecond(type, first))
            //{
            //    return InvokeFirstIEquatableEqualsSecond(actual, expected);
            //}
            return expected.Equals(actual);
        }


        private bool EnumerablesEqual(IEnumerable expected, IEnumerable actual)
        {
            //if (this.recursionDetector.CheckRecursion(expected, actual))
            //{
            //    return false;
            //}
            IEnumerator enumerator = expected.GetEnumerator();
            IEnumerator enumerator2 = actual.GetEnumerator();
            int num = 0;
            while (true)
            {
                bool flag = enumerator.MoveNext();
                bool flag2 = enumerator2.MoveNext();
                if (!(flag || flag2))
                {
                    return true;
                }
                if ((flag != flag2) || !this.ObjectsEqual(enumerator.Current, enumerator2.Current))
                {
                    FailurePoint item = new FailurePoint
                    {
                        Position = num,
                        ExpectedHasData = flag
                    };
                    if (flag)
                    {
                        item.ExpectedValue = enumerator.Current;
                    }
                    item.ActualHasData = flag2;
                    if (flag2)
                    {
                        item.ActualValue = enumerator2.Current;
                    }
                    this.failurePoints.Insert(0, item);
                    return false;
                }
                num++;
            }
        }

        public EqualConstraint Using(IEqualityComparer comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            this.comparer = comparer;
            return this;
        }

        private class FailurePointList : ArrayList
        {
        }
    }


    public abstract class BasicConstraint : Constraint
    {
        private readonly string description;
        private readonly object expected;

        protected BasicConstraint(object expected, string description)
        {
            this.expected = expected;
            this.description = description;
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            if ((actual == null) && (this.expected == null))
            {
                return true;
            }
            if ((actual == null) || (this.expected == null))
            {
                return false;
            }
            return this.expected.Equals(actual);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write(this.description);
        }
    }
    
    
    public class NullConstraint : BasicConstraint
        {
            public NullConstraint()
                : base(null, "null")
            {
            }
        }

    
    public class FailurePoint
    {
        public bool ActualHasData;
        public object ActualValue;
        public bool ExpectedHasData;
        public object ExpectedValue;
        public int Position;
    }


    public abstract class ConstraintOperator
    {
        protected int left_precedence;
        private object leftContext;
        protected int right_precedence;
        private object rightContext;

        protected ConstraintOperator()
        {
        }

        public abstract void Reduce(ConstraintBuilder.ConstraintStack stack);

        public object LeftContext
        {
            get
            {
                return this.leftContext;
            }
            set
            {
                this.leftContext = value;
            }
        }

        public virtual int LeftPrecedence
        {
            get
            {
                return this.left_precedence;
            }
        }

        public object RightContext
        {
            get
            {
                return this.rightContext;
            }
            set
            {
                this.rightContext = value;
            }
        }

        public virtual int RightPrecedence
        {
            get
            {
                return this.right_precedence;
            }
        }
    }


    public abstract class ConstraintExpressionBase
    {
        protected ConstraintBuilder builder;

        public ConstraintExpressionBase()
        {
            this.builder = new ConstraintBuilder();
        }

        public ConstraintExpressionBase(ConstraintBuilder builder)
        {
            this.builder = builder;
        }

        public Constraint Append(Constraint constraint)
        {
            this.builder.Append(constraint);
            return constraint;
        }

        public ConstraintExpression Append(ConstraintOperator op)
        {
            this.builder.Append(op);
            return (ConstraintExpression)this;
        }

        public ResolvableConstraintExpression Append(SelfResolvingOperator op)
        {
            this.builder.Append(op);
            return new ResolvableConstraintExpression(this.builder);
        }

        //public override string ToString()
        //{
        //    return this.builder.Resolve().ToString();
        //}
    }

    public class ConstraintExpression : ConstraintExpressionBase
    {
        public ConstraintExpression()
        {
        }

        public ConstraintExpression(ConstraintBuilder builder)
            : base(builder)
        {
        }

        public EqualConstraint EqualTo(object expected)
        {
            return (EqualConstraint)base.Append(new EqualConstraint(expected));
        }


        public StartsWithConstraint StartsWith(string expected)
        {
            return (StartsWithConstraint)base.Append(new StartsWithConstraint(expected));
        }


        public ConstraintExpression Not
        {
            get
            {
                return base.Append(new NotOperator());
            }
        }


        public NullConstraint Null
        {
            get
            {
                return (NullConstraint)base.Append(new NullConstraint());
            }
        }
        
        
        public ResolvableConstraintExpression Message
        {
            get
            {
                return this.Property("Message");
            }
        }


        public ResolvableConstraintExpression Property(string name)
        {
            return base.Append((SelfResolvingOperator)new PropOperator(name));
        }

        
        public InstanceOfTypeConstraint InstanceOf(Type expectedType)
        {
            return (InstanceOfTypeConstraint)base.Append(new InstanceOfTypeConstraint(expectedType));
        }

        public ExactTypeConstraint TypeOf(Type expectedType)
        {
            return (ExactTypeConstraint)base.Append(new ExactTypeConstraint(expectedType));
        }

        public SubstringConstraint Contains(string expected)
        {
            return (SubstringConstraint)base.Append(new SubstringConstraint(expected));
        }
    }


    public class ResolvableConstraintExpression : ConstraintExpression, IResolveConstraint
    {
        public ResolvableConstraintExpression()
        {
        }

        public ResolvableConstraintExpression(ConstraintBuilder builder)
            : base(builder)
        {
        }

        Constraint IResolveConstraint.Resolve()
        {
            return base.builder.Resolve();
        }

    }


    public abstract class PrefixOperator : ConstraintOperator
    {
        protected PrefixOperator()
        {
        }

        public abstract Constraint ApplyPrefix(Constraint constraint);
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            stack.Push(this.ApplyPrefix(stack.Pop()));
        }
    }


    public class NotOperator : PrefixOperator
    {
        public NotOperator()
        {
            base.left_precedence = base.right_precedence = 1;
        }

        public override Constraint ApplyPrefix(Constraint constraint)
        {
            return new NotConstraint(constraint);
        }
    }

    public abstract class PrefixConstraint : Constraint
    {
        protected Constraint baseConstraint;

        protected PrefixConstraint(IResolveConstraint resolvable)
            : base(resolvable)
        {
            if (resolvable != null)
            {
                this.baseConstraint = resolvable.Resolve();
            }
        }
    }

    public class NotConstraint : PrefixConstraint
    {
        public NotConstraint(Constraint baseConstraint)
            : base(baseConstraint)
        {
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            return !base.baseConstraint.Matches(actual);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            base.baseConstraint.WriteActualValueTo(writer);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("not");
            base.baseConstraint.WriteDescriptionTo(writer);
        }
    }


    public abstract class TypeConstraint : Constraint
    {
        protected readonly Type expectedType;

        protected TypeConstraint(Type type)
            : base(type)
        {
            this.expectedType = type;
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteActualValue((base.actual == null) ? null : base.actual.GetType());
        }
    }


    public class ExactTypeConstraint : TypeConstraint
    {
        public ExactTypeConstraint(Type type)
            : base(type)
        {
            //base.DisplayName = "typeof";
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            return ((actual != null) && (actual.GetType() == base.expectedType));
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteExpectedValue(base.expectedType);
        }
    }


    public class InstanceOfTypeConstraint : TypeConstraint
    {
        public InstanceOfTypeConstraint(Type type)
            : base(type)
        {
            //base.DisplayName = "instanceof";
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            return ((actual != null) && base.expectedType.IsInstanceOfType(actual));
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("instance of");
            writer.WriteExpectedValue(base.expectedType);
        }
    }


    public abstract class SelfResolvingOperator : ConstraintOperator
    {
        protected SelfResolvingOperator()
        {
        }
    }


    public class ThrowsOperator : SelfResolvingOperator
    {
        public ThrowsOperator()
        {
            base.left_precedence = 1;
            base.right_precedence = 100;
        }

        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            //if ((base.RightContext == null) || (base.RightContext is BinaryOperator))
            if (base.RightContext == null)
            {
                stack.Push(new ThrowsConstraint(null));
            }
            else
            {
                stack.Push(new ThrowsConstraint(stack.Pop()));
            }
        }
    }


    public class ThrowsConstraint : PrefixConstraint
    {
        private Exception caughtException;

        public ThrowsConstraint(Constraint baseConstraint)
            : base(baseConstraint)
        {
        }

        protected override string GetStringRepresentation()
        {
            if (base.baseConstraint == null)
            {
                return "<throws>";
            }
            return base.GetStringRepresentation();
        }

        public override bool Matches(object actual)
        {
            TestDelegate del = (TestDelegate)actual;
            try
            {
                del.Invoke();
                return false;
            }
            catch (Exception exception)
            {
                this.caughtException = exception;
            }
            return ((base.baseConstraint == null) || base.baseConstraint.Matches(this.caughtException));
        }

        //public override bool Matches<T>(ActualValueDelegate<T> del)
        //{
        //    return this.Matches(new GenericInvocationDescriptor<T>(del));
        //}

        public override void WriteActualValueTo(MessageWriter writer)
        {
            if (this.caughtException == null)
            {
                writer.Write("no exception thrown");
            }
            else if (base.baseConstraint != null)
            {
                base.baseConstraint.WriteActualValueTo(writer);
            }
            else
            {
                writer.WriteActualValue(this.caughtException);
            }
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            if (base.baseConstraint == null)
            {
                writer.WritePredicate("an exception");
            }
            else
            {
                base.baseConstraint.WriteDescriptionTo(writer);
            }
        }

        public Exception ActualException
        {
            get
            {
                return this.caughtException;
            }
        }
    }


    public class ThrowsNothingConstraint : Constraint
    {
        private Exception caughtException;

        public override bool Matches(object actual)
        {
            TestDelegate del = (TestDelegate)actual;
            try
            {
                del.Invoke();
            }
            catch (Exception exception)
            {
                this.caughtException = exception;
            }
            return (this.caughtException == null);
        }

        //public override bool Matches<T>(ActualValueDelegate<T> del)
        //{
        //    return this.Matches(new GenericInvocationDescriptor<T>(del));
        //}

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteLine(StringEx.Format(" ({0})", this.caughtException.Message));
            writer.Write(this.caughtException.StackTrace);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write(StringEx.Format("No Exception to be thrown", new object[0]));
        }
    }


    public abstract class BinaryOperator : ConstraintOperator
    {
        protected BinaryOperator()
        {
        }

        public abstract Constraint ApplyOperator(Constraint left, Constraint right);
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            Constraint right = stack.Pop();
            Constraint left = stack.Pop();
            stack.Push(this.ApplyOperator(left, right));
        }

        public override int LeftPrecedence
        {
            get
            {
                return ((base.RightContext is CollectionOperator) ? (base.LeftPrecedence + 10) : base.LeftPrecedence);
            }
        }

        public override int RightPrecedence
        {
            get
            {
                return ((base.RightContext is CollectionOperator) ? (base.RightPrecedence + 10) : base.RightPrecedence);
            }
        }
    }

    
    public class AndOperator : BinaryOperator
    {
        public AndOperator()
        {
            base.left_precedence = base.right_precedence = 2;
        }

        public override Constraint ApplyOperator(Constraint left, Constraint right)
        {
            return new AndConstraint(left, right);
        }
    }


    public abstract class CollectionOperator : PrefixOperator
    {
        protected CollectionOperator()
        {
            base.left_precedence = 1;
            base.right_precedence = 10;
        }
    }


    public abstract class BinaryConstraint : Constraint
    {
        protected Constraint left;
        protected Constraint right;

        protected BinaryConstraint(Constraint left, Constraint right)
            : base(left, right)
        {
            this.left = left;
            this.right = right;
        }
    }


    public class AndConstraint : BinaryConstraint
    {
        private FailurePoint failurePoint;

        public AndConstraint(Constraint left, Constraint right)
            : base(left, right)
        {
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            this.failurePoint = base.left.Matches(actual) ? (base.right.Matches(actual) ? FailurePoint.None : FailurePoint.Right) : FailurePoint.Left;
            return (this.failurePoint == FailurePoint.None);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            switch (this.failurePoint)
            {
                case FailurePoint.Left:
                    base.left.WriteActualValueTo(writer);
                    break;

                case FailurePoint.Right:
                    base.right.WriteActualValueTo(writer);
                    break;

                default:
                    base.WriteActualValueTo(writer);
                    break;
            }
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            base.left.WriteDescriptionTo(writer);
            writer.WriteConnector("and");
            base.right.WriteDescriptionTo(writer);
        }

        private enum FailurePoint
        {
            None,
            Left,
            Right
        }
    }


    public class PropOperator : SelfResolvingOperator
    {
        private readonly string name;

        public PropOperator(string name)
        {
            this.name = name;
            base.left_precedence = base.right_precedence = 1;
        }

        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if ((base.RightContext == null) || (base.RightContext is BinaryOperator))
            {
                throw new NotSupportedException("PropertyExistsConstraint not supported");
                // stack.Push(new PropertyExistsConstraint(this.name));
            }
            else
            {
                stack.Push(new PropertyConstraint(this.name, stack.Pop()));
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }


    public class PropertyConstraint : PrefixConstraint
    {
        private readonly string name;
        private object propValue;

        public PropertyConstraint(string name, Constraint baseConstraint)
            : base(baseConstraint)
        {
            this.name = name;
        }

        protected override string GetStringRepresentation()
        {
            return StringEx.Format("<property {0} {1}>", this.name, base.baseConstraint);
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }
            Type type = actual as Type;
            if (type == null)
            {
                type = actual.GetType();
            }
            MethodInfo methodInfo = type.GetMethod("get_" + this.name, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new InvalidOperationException(StringEx.Format("Property get_{0} was not found", this.name));
            }
            this.propValue = methodInfo.Invoke(actual, null);
            return base.baseConstraint.Matches(this.propValue);
            //Microsoft.SPOT.Debug.Print(method.Name + "virtual=" + method.IsVirtual.ToString() + "memberType=" + method.MemberType.ToString() + "abstract=" + method.IsAbstract);
            //MethodInfo[] methods = type.GetMethods(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            //foreach (MethodInfo m in methods)
            //{
            //    Microsoft.SPOT.Debug.Print(m.Name + "virtual=" + m.IsVirtual.ToString() + "memberType=" + m.MemberType.ToString() + "abstract=" + m.IsAbstract);
            //}
            //PropertyInfo property = null; // type.GetProperty(this.name, BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            //if (property == null)
            //{
            //    throw new ArgumentException(StringEx.Format("Property {0} was not found", this.name), "name");
            //}
            //this.propValue = property.GetValue(actual, null);
            //return base.baseConstraint.Matches(this.propValue);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteActualValue(this.propValue);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("property " + this.name);
            if (base.baseConstraint != null)
            {
                if (base.baseConstraint is EqualConstraint)
                {
                    writer.WritePredicate("equal to");
                }
                base.baseConstraint.WriteDescriptionTo(writer);
            }
        }
    }


    public class StartsWithConstraint : StringConstraint
    {
        public StartsWithConstraint(string expected)
            : base(expected)
        {
        }

        protected override bool Matches(string actual)
        {
            if (base.caseInsensitive)
            {
                //return actual.ToLower().StartsWith(base.expected.ToLower());
                return (actual.ToLower().IndexOf(base.expected) == 0);
            }
            else
            {
                //return actual.StartsWith(base.expected);
                return (actual.IndexOf(base.expected) == 0);
            }
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("String starting with");
            writer.WriteExpectedValue(base.expected);
            if (base.caseInsensitive)
            {
                writer.WriteModifier("ignoring case");
            }
        }
    }


    public abstract class StringConstraint : Constraint
    {
        protected bool caseInsensitive;
        protected readonly string expected;

        protected StringConstraint(string expected)
            : base(expected)
        {
            this.expected = expected;
        }

        public override bool Matches(object actual)
        {
            base.actual = actual;
            string str = actual as string;
            return ((str != null) && this.Matches(str));
        }

        protected abstract bool Matches(string actual);

        public StringConstraint IgnoreCase
        {
            get
            {
                this.caseInsensitive = true;
                return this;
            }
        }
    }


    public class SubstringConstraint : StringConstraint
    {
        public SubstringConstraint(string expected)
            : base(expected)
        {
        }

        protected override bool Matches(string actual)
        {
            if (base.caseInsensitive)
            {
                return (actual.ToLower().IndexOf(base.expected.ToLower()) >= 0);
            }
            return (actual.IndexOf(base.expected) >= 0);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("String containing");
            writer.WriteExpectedValue(base.expected);
            if (base.caseInsensitive)
            {
                writer.WriteModifier("ignoring case");
            }
        }
    }

}
