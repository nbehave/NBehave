using System;
using System.Reflection;

namespace NBehave.Spec.Framework
{
    public delegate void MethodThatThrows();

    public delegate void NewSpecificationHandler(Specify spec);

    public delegate void RunnersStopListeningHandler();

    public delegate void RunnersStartListeningHandler();

    public class SpecifyContext<T>
    {
        private readonly T _value;

        public SpecifyContext(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }

        public static implicit operator T(SpecifyContext<T> sc)
        {
            return sc.Value;
        }

        public static explicit operator SpecifyContext<T>(T value)
        {
            return new SpecifyContext<T>(value);
        }

        public static SpecifyContext<bool> ShouldBeFalse<T>(T value)
            where T : SpecifyContext<bool>
        {
            comparer = new BooleanComparer(false, Actual);

            return comparer as BooleanComparer;
        }
    }

    public class Specify
    {
        private static bool broadcastNextSpec = true;
        internal static Specify LastSpecification = null;
        internal object Actual = null;
        private Comparer comparer = null;

        private Specify(object result)
        {
            Actual = result;

            if (NewSpecification != null && broadcastNextSpec)
                NewSpecification(this);

            broadcastNextSpec = true;

            LastSpecification = this;
        }

        public Failure LastFailure
        {
            get { return comparer.Failure; }
        }

        public static event NewSpecificationHandler NewSpecification;
        public static event RunnersStopListeningHandler StopListening;
        public static event RunnersStartListeningHandler StartListening;

        internal static void DontBroadcastNextSpec()
        {
            broadcastNextSpec = false;
        }

        internal static void RunnersStopListening()
        {
            if (StopListening != null)
                StopListening();
        }

        internal static void RunnersStartListening()
        {
            if (StartListening != null)
                StartListening();
        }

        public static SpecifyContext<T> That<T>(T value)
        {
            return new SpecifyContext<T>(value);
        }

        public static Specify ThrownBy(MethodThatThrows method)
        {
            Specify r = null;

            try
            {
                method.DynamicInvoke(null);
                r = new Specify(null);
            }
            catch (TargetInvocationException outerException)
            {
                r = new Specify(outerException.InnerException);
            }

            return r;
        }

        public BooleanComparer ShouldBeFalse()
        {
            comparer = new BooleanComparer(false, Actual);

            return comparer as BooleanComparer;
        }

        public ObjectComparer ShouldEqual(object expected)
        {
            comparer = new ObjectComparer(expected, Actual);

            return comparer as ObjectComparer;
        }

        public DoubleComparer ShouldEqual(double expected)
        {
            comparer = new DoubleComparer(expected, Actual);

            return comparer as DoubleComparer;
        }

        public Int32Comparer ShouldEqual(int expected)
        {
            comparer = new Int32Comparer(expected, Actual);

            return comparer as Int32Comparer;
        }

        public NotNullComparer ShouldNotBeNull()
        {
            comparer = new NotNullComparer(Actual);

            return comparer as NotNullComparer;
        }

        public NullComparer ShouldBeNull()
        {
            comparer = new NullComparer(Actual);

            return comparer as NullComparer;
        }

        public ReferentialEqualityComparer ShouldBeTheSameAs(object expected)
        {
            comparer = new ReferentialEqualityComparer(expected, Actual);

            return comparer as ReferentialEqualityComparer;
        }

        public ReferentialInequalityComparer ShouldNotBeTheSameAs(object expected)
        {
            comparer = new ReferentialInequalityComparer(expected, Actual);

            return comparer as ReferentialInequalityComparer;
        }

        public BooleanComparer ShouldBeTrue()
        {
            comparer = new BooleanComparer(true, Actual);

            return comparer as BooleanComparer;
        }

        public TypeComparer ShouldBeOfType(Type type)
        {
            comparer = new TypeComparer(type, Actual);

            return comparer as TypeComparer;
        }

        public bool ComparesOK()
        {
            if (comparer != null)
                return comparer.ComparesOK();
            else
                return false;
        }
    }
}