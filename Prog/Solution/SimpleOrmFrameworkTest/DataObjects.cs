using System;
using System.Collections.Generic;
using SimpleOrmFramework;
using System.Reflection;
using System.Drawing;

namespace SimpleOrmFrameworkTest
{

    class PointCustomBasicTypeProvider : ICustomBasicTypeProvider
    {
        #region ICustomBasicTypeProvider Members

        public MemberInfo[] GetDataProperties()
        {
            Type type = typeof(Point);
            return new PropertyInfo[] { type.GetProperty("X"), type.GetProperty("Y") };
        }

        public object CreateInstance()
        {
            return new Point();
        }

        #endregion
    }

    [DataObject]
    [CustomBasicTypeProvider(typeof(Point), typeof(PointCustomBasicTypeProvider))]
    public class ObjWithPoints : BaseDataObject
    {
        private Point mPoint;
        private List<Point> mPoints;

        [DataProperty]
        public Point Point
        {
            get { return mPoint; }
            set { mPoint = value; }
        }

        [DataListProperty]
        public List<Point> Points
        {
            get { return mPoints; }
            set { mPoints = value; }
        }

        public ObjWithPoints()
        {
            mPoints = new List<Point>();
        }   
    }

    [DataObject]
    public class PersonB : Person
    {
        string mTextFromB;

        [DataProperty]
        public string TextFromB
        {
            get { return mTextFromB; }
            set { mTextFromB = value; }
        }

        public PersonB(string name, string address, Account account, string textFromB)
            : base(name, address, account)
        {
            mTextFromB = textFromB;
        }

        protected PersonB()
        { }
    }


    [DataObject]
    public class PersonA : PersonB
    {
        public static string AfterLoadText = "AfterLoadText";

        public enum TestEnum
        {
            test1,
            test2

        }

        string mTextFromA;
        TestEnum mEnumTest;

        [DataProperty(NotNull = true)]
        public string TextFromA
        {
            get { return mTextFromA; }
            set { mTextFromA = value; }
        }

        [DataProperty]
        public TestEnum EnumTest
        {
            get { return mEnumTest; }
            set { mEnumTest = value; }
        }

        public PersonA(string name, string address, Account account, string textFromA, string textFromB)
            : base(name, address, account, textFromB)
        {
            mTextFromA = textFromA;
        }

        protected PersonA()
        { }

        protected override void AfterLoadImpl(OrmManager ormManager)
        {
            TextFromA = AfterLoadText;
        }
    }

    [DataObject]
    public class Person : BaseDataObject
    {
        string mName;
        string mAddress;
        Account mAccount;
        List<Person> mChildrens = new List<Person>();

        [DataProperty(NotNull = true, Unique = true)]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        [DataProperty]
        public string Address
        {
            get { return mAddress; }
            set { mAddress = value; }
        }

        [DataProperty]
        public Account Account
        {
            get { return mAccount; }
            set { mAccount = value; }
        }

        [DataListProperty]
        public List<Person> Childs
        {
            get { return mChildrens; }
            set { mChildrens = value; }
        }

        public Person(string name, string address, Account account)
        {
            mName = name;
            mAddress = address;
            mAccount = account;
        }

        protected Person()
        { }

        public override string ToString()
        {
            return Name;
        }
    }

    [DataObject]
    public class Account : BaseDataObject
    {
        string mName;
        string mPassword;

        [DataProperty(NotNull = true, Unique = true)]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        [DataProperty]
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        public Account(string name, string password)
        {
            mName = name;
            mPassword = password;
        }

        private Account()
        { }
    }

    [DataObject]
    public class WeakNode : BaseDataObject
    {
        private WeakNode mWeakChild;
        private WeakNode mNonWeakChild;

        [DataProperty(WeakReference=true)]
        public WeakNode WeakChild
        {
            get { return mWeakChild; }
            set { mWeakChild = value; }
        }

        [DataProperty]
        public WeakNode NonWeakChild
        {
            get { return mNonWeakChild; }
            set { mNonWeakChild = value; }
        }
    }

    [DataObject(MaxCachedCnt=3)]
    public class ObjectWithCacheLimit : BaseDataObject
    {
        private string mText;
        Account mAccount;

        [DataProperty]
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }
        [DataProperty]
        public Account Account
        {
            get { return mAccount; }
            set { mAccount = value; }
        }

        public ObjectWithCacheLimit(string text, Account account)
        {
            mText = text;
            mAccount = account;
        }

        private ObjectWithCacheLimit()
        {
        }
    }

    [DataObject]
    public class ObjectWithDerivedList : BaseDataObject
    {
        MyList mList = new MyList();
        
        [DataListProperty]
        public MyList List
        {
            get { return mList; }
            private set { mList = value; }
        }
    }

    //list
    public class MyList : List<int>
    {
    }
}