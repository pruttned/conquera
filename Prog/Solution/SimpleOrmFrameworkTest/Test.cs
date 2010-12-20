//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SimpleOrmFramework;
using System.IO;
using System.Drawing;
using System.Data.SQLite;

namespace SimpleOrmFrameworkTest
{
    [TestFixture]
    public class Test
    {
        OrmManager mOrmManager;

        [SetUp]
        public void Setup()
        {
            if (File.Exists("file.db"))
            {
                File.Delete("file.db");
            }

            mOrmManager = new OrmManager("Data Source=file.db;Version=3;New=False;Compress=False");
        }

        [TearDown]
        public void End()
        {
            mOrmManager.Dispose();
        }

        [Test(Description = "Person with list of child persons - Save and Load")]
        public void DataObject1nReference()
        {
            PersonA jozo = new PersonA("Jozko mrkvicka2", "Niekde v BA", new Account("JM2", "JM123"), "text from A", "text from B");
            jozo.EnumTest = PersonA.TestEnum.test2;            
            jozo.Childs.Add(new Person("Ferko mrkvicka", "Niekde v BA", new Account("FM", "FM123")));
            jozo.Childs.Add(new Person("Anca mrkvickova", "Niekde v BA", new Account("AM", "AM123")));
            mOrmManager.SaveObject(jozo);
            PersonA jozo2 = mOrmManager.LoadObject<PersonA>(jozo.Id);
            //PersonA jozo3 = mOrmManager.LoadObject<PersonA>(jozo.Id);

            CheckPersonEquality(jozo, jozo2);
            //CheckPersonEquality(jozo, jozo3);

            Dictionary<long, IDataObject> cache = GetCachedObjects(mOrmManager);
            Assert.That(cache.Count, Is.EqualTo(6), "cache.Count");
            Assert.That(cache.Values, Has.Some.TypeOf<Account>(), "cache.Values contains Accounts");
            Assert.That(cache.Values, Has.Some.TypeOf<Person>(), "cache.Values contains Persons");
        }

        [Test(Description = "Should throw because of uninitialized weak reference")]
        [ExpectedException(typeof(SofException))]
        public void ThrowNoninitializedWeakReference()
        {
            WeakNode node1 = new WeakNode();
            WeakNode node2 = new WeakNode();

            node1.WeakChild = node2;

            mOrmManager.SaveObject(node1);
        }

        [Test(Description = "Saving and loading object with initialized weak reference")]
        public void SaveLoadObjectWithWeakReference()
        {
            WeakNode node1 = new WeakNode();
            WeakNode node2 = new WeakNode();

            node1.WeakChild = node2;

            mOrmManager.SaveObject(node2);
            mOrmManager.SaveObject(node1);

            WeakNode node2_1 = mOrmManager.LoadObject<WeakNode>(node2.Id);
            WeakNode node1_1 = mOrmManager.LoadObject<WeakNode>(node1.Id);

            Assert.That(node1_1.WeakChild, Is.Not.Null, "node1_1.WeakChild");
            Assert.That(node1_1.WeakChild.Id, Is.EqualTo(node2.Id), "node1_1.WeakChild");
        }


        [Test(Description = "Test of the load cache during loading of circular weak referenced objets")]
        public void CircularReferenceLoadCache()
        {
            WeakNode node1 = new WeakNode();
            WeakNode node2 = new WeakNode();

            node1.NonWeakChild = node2;
            node2.WeakChild = node1;

            mOrmManager.SaveObject(node1);

            WeakNode node1_1 = mOrmManager.LoadObject<WeakNode>(node1.Id);
            WeakNode node1_2 = node1_1.NonWeakChild;

            Assert.That(node1_1.NonWeakChild, Is.Not.Null, "node1_1.NonWeakChild");
            Assert.That(node1_1.NonWeakChild.Id, Is.EqualTo(2), "node1_1.NonWeakChild");
            Assert.That(node1_2.WeakChild, Is.Not.Null, "node1_2.WeakChild");
            Assert.That(node1_1, Is.SameAs(node1_2.WeakChild), "node1_2.WeakChild");
        }

        [Test(Description = "Search")]
        public void Search()
        {
            PersonA person1 = new PersonA("Jozko Mrkvicka", "Niekde v BA", new Account("JM2", "JM123"), "text from A", "text from B");
            Person person2 = new Person("Ferko Kalerab", "Neviem kde", new Account("FM", "FM123"));
            mOrmManager.SaveObject(person1);
            mOrmManager.SaveObject(person2);

            Person[] p1 = mOrmManager.LoadObjects<Person>("Name='Jozko Mrkvicka'");
            Person[] p2 = mOrmManager.LoadObjects<Person>("Account.Name='FM' AND Account.Password='FM123'");
            Person[] p3 = mOrmManager.LoadObjects<Person>("Account.Name='FM' AND Account.Password='JM123'");
            PersonA[] p4 = mOrmManager.LoadObjects<PersonA>("TextFromB='text from B'");

            Assert.That(p1, Has.Length.EqualTo(1), "Name='Jozko Mrkvicka'  Count");
            Assert.That(p2, Has.Length.EqualTo(1), "Account.Name='FM' AND Account.Password='FM123'  Count");
            Assert.That(p3, Has.Length.EqualTo(0), "Account.Name='FM' AND Account.Password='JM123'  Count");
            Assert.That(p4, Has.Length.EqualTo(1), "TextFromB='text from B'");
            Assert.That(p1[0].Name, Is.EqualTo(person1.Name), "Name='Jozko Mrkvicka'  Name");
            Assert.That(p2[0].Name, Is.EqualTo(person2.Name), "Account.Name='FM' AND Account.Password='FM123'  Name");
        }

        [Test(Description = "Delete object with strong references")]
        public void DeleteObjectWithStrongReferences()
        {
            PersonA jozo = new PersonA("Jozko mrkvicka2", "Niekde v BA", new Account("JM2", "JM123"), "text from A", "text from B");
            jozo.Childs.Add(new Person("Ferko mrkvicka", "Niekde v BA", new Account("FM", "FM123")));
            jozo.Childs.Add(new Person("Anca mrkvickova", "Niekde v BA", new Account("AM", "AM123")));
            mOrmManager.SaveObject(jozo);
            PersonA jozo2 = mOrmManager.LoadObject<PersonA>(jozo.Id); //fill cache

            mOrmManager.DeleteObject(jozo.Id);

            Assert.That(mOrmManager.ExistsObjectWithId(jozo.Id), Is.False, "mOrmManager.ExistsObjectWithId(jozo.Id)");

            //cache must be empty
            Dictionary<long, IDataObject> cache = GetCachedObjects(mOrmManager);
            Assert.That(cache.Count, Is.EqualTo(0), "cache.Count ");
            Assert.That(GetRowCount("SimpleOrmFrameworkTest.Person.Childs"), Is.EqualTo(0), "SimpleOrmFrameworkTest.Person.Childs");
        }

        [Test(Description = "Delete object with weak reference")]
        public void DeleteObjectWithWeakReferences()
        {
            WeakNode node1 = new WeakNode();
            WeakNode node2 = new WeakNode();

            node1.WeakChild = node2;

            mOrmManager.SaveObject(node2);
            mOrmManager.SaveObject(node1);

            mOrmManager.DeleteObject(node1.Id);

            Assert.That(mOrmManager.ExistsObjectWithId(node1.Id), Is.False, "mOrmManager.ExistsObjectWithId(node1.Id)");
            Assert.That(mOrmManager.ExistsObjectWithId(node2.Id), Is.True, "mOrmManager.ExistsObjectWithId(node2.Id)");
        }


        [Test(Description = "Update object with strong references")]
        public void UpdateObjectWithStrongReferences()
        {
            PersonA jozo = new PersonA("Jozko mrkvicka2", "Niekde v BA", new Account("JM2", "JM123"), "text from A", "text from B");
            jozo.Childs.Add(new Person("Ferko mrkvicka", "Niekde v BA", new Account("FM", "FM123")));
            Person anca = new Person("Anca mrkvickova", "Niekde v BA", new Account("AM", "AM123"));
            jozo.Childs.Add(anca);
            mOrmManager.SaveObject(jozo);

            jozo.Name = "NewName";
            jozo.Childs[0].Name = "NewChild";
            jozo.Account.Name= "NewAccountName";
            jozo.Childs.RemoveAt(1);
            mOrmManager.SaveObject(jozo);

            PersonA jozo2 = mOrmManager.LoadObject<PersonA>(jozo.Id);

            CheckPersonEquality(jozo, jozo2);

            Assert.That(mOrmManager.ExistsObjectWithId(anca.Id), Is.False, "mOrmManager.ExistsObjectWithId(anca.Id)");

            Dictionary<long, IDataObject> cache = GetCachedObjects(mOrmManager);
            Assert.That(cache.Count, Is.EqualTo(4), "cache.Count ");
        }

        [Test(Description = "Update object with weak")]
        public void UpdateObjectWithWeakReference()
        {
            WeakNode node1 = new WeakNode();
            WeakNode node2 = new WeakNode();
            WeakNode node3 = new WeakNode();

            node1.WeakChild = node2;

            mOrmManager.SaveObject(node2);
            mOrmManager.SaveObject(node1);
            mOrmManager.SaveObject(node3);

            node1.WeakChild = node3;

            mOrmManager.SaveObject(node3);
            mOrmManager.SaveObject(node1);

            WeakNode node1_1 = mOrmManager.LoadObject<WeakNode>(node1.Id);

            Assert.That(mOrmManager.ExistsObjectWithId(node2.Id), Is.True, "mOrmManager.ExistsObjectWithId(node2.Id)");
            Assert.That(node1_1.WeakChild.Id, Is.EqualTo(node3.Id), "node1_1.WeakChild");
        }

        [Test(Description = "Save and load object with Point and List<Point> properties")]
        public void SaveLoadObjWithPoints()
        {
            ObjWithPoints obj = new ObjWithPoints();
            obj.Point = new Point(1, 2);
            obj.Points.Add(new Point(2, 3));
            obj.Points.Add(new Point(3, 4));

            mOrmManager.SaveObject(obj);

            ObjWithPoints obj1 = mOrmManager.LoadObject<ObjWithPoints>(obj.Id);

            Assert.That(obj1.Point, Is.EqualTo(obj.Point), "obj.Point");
            Assert.That(obj1.Points, Has.Count.EqualTo(obj.Points.Count), "obj.Points.Count");

            Assert.That(obj1.Points[0], Is.EqualTo(obj.Points[0]), "obj1.Points[0]");
            Assert.That(obj1.Points[1], Is.EqualTo(obj.Points[1]), "obj1.Points[1]");
        }

        [Test(Description = "Save and delete object with Point and List<Point> properties")]
        public void DeleteObjWithPoints()
        {
            ObjWithPoints obj = new ObjWithPoints();
            obj.Point = new Point(1, 2);
            obj.Points.Add(new Point(2, 3));
            obj.Points.Add(new Point(3, 4));

            mOrmManager.SaveObject(obj);

            mOrmManager.DeleteObject(obj.Id);

            Assert.That(mOrmManager.ExistsObjectWithId(obj.Id), Is.False, "mOrmManager.ExistsObjectWithId(obj.Id)");
            Assert.That(GetRowCount("SimpleOrmFrameworkTest.ObjWithPoints.Points"), Is.EqualTo(0), "GetRowCount(SimpleOrmFrameworkTest.ObjWithPoints.Points)");
        }

        [Test(Description = "Save and update object with Point and List<Point> properties")]
        public void UpdateObjWithPoints()
        {
            ObjWithPoints obj = new ObjWithPoints();
            obj.Point = new Point(1, 2);
            obj.Points.Add(new Point(2, 3));
            obj.Points.Add(new Point(3, 4));

            mOrmManager.SaveObject(obj);

            obj.Point = new Point(11, 22);
            obj.Points.Add(new Point(22, 33));
            obj.Points.RemoveAt(0);

            mOrmManager.SaveObject(obj);

            ObjWithPoints obj1 = mOrmManager.LoadObject<ObjWithPoints>(obj.Id);

            Assert.That(GetRowCount("SimpleOrmFrameworkTest.ObjWithPoints.Points"), Is.EqualTo(obj.Points.Count), "GetRowCount(SimpleOrmFrameworkTest.ObjWithPoints.Points)");
            Assert.That(obj1.Points, Has.Count.EqualTo(obj.Points.Count), "obj.Points.Count");
            Assert.That(obj1.Points[0], Is.EqualTo(obj.Points[0]), "obj1.Points[0]");
            Assert.That(obj1.Points[1], Is.EqualTo(obj.Points[1]), "obj1.Points[1]");
        }

        [Test(Description = "Limiting number of objects of a specified type stored in cache by it")]
        public void LimitCacheByType()
        {
            ObjectWithCacheLimit o1 = new ObjectWithCacheLimit("o1", new Account("ao1", "po1"));
            ObjectWithCacheLimit o2 = new ObjectWithCacheLimit("o2", new Account("ao2", "po2"));
            ObjectWithCacheLimit o3 = new ObjectWithCacheLimit("o3", new Account("ao3", "po3"));
            ObjectWithCacheLimit o4 = new ObjectWithCacheLimit("o4", new Account("ao4", "po4"));
            ObjectWithCacheLimit o5 = new ObjectWithCacheLimit("o5", new Account("ao5", "po5"));

            mOrmManager.SaveObject(o1);
            mOrmManager.SaveObject(o2);
            mOrmManager.SaveObject(o3);
            mOrmManager.SaveObject(o4);
            mOrmManager.SaveObject(o5);

            Dictionary<long, IDataObject> cache = GetCachedObjects(mOrmManager);

            mOrmManager.LoadObject(o1.Id);
            Assert.That(cache.Count, Is.EqualTo(2), "cache.Count");
            mOrmManager.LoadObject(o2.Id);
            Assert.That(cache.Count, Is.EqualTo(4), "cache.Count");
            mOrmManager.LoadObject(o3.Id);
            Assert.That(cache.Count, Is.EqualTo(6), "cache.Count");

            mOrmManager.LoadObject(o4.Id);
            Assert.That(cache.Count, Is.EqualTo(6), "cache.Count");
            Assert.That(cache.ContainsKey(o4.Id), Is.True, "cache.ContainsKey(o4.Id)");
            Assert.That(cache.ContainsKey(o1.Id), Is.False, "cache.ContainsKey(o1.Id)");

            mOrmManager.LoadObject(o5.Id);
            Assert.That(cache.Count, Is.EqualTo(6), "cache.Count");
            Assert.That(cache.ContainsKey(o5.Id), Is.True, "cache.ContainsKey(o5.Id)");
            Assert.That(cache.ContainsKey(o2.Id), Is.False, "cache.ContainsKey(o2.Id)");
        }


        [Test(Description = "List property that is derived from List<T>")]
        public void ObjectWithDerivedListSaveLoadUpdate()
        {
            ObjectWithDerivedList o1 = new ObjectWithDerivedList();
            o1.List.Add(1);
            o1.List.Add(2);
            o1.List.Add(3);

            mOrmManager.SaveObject(o1);
            
            o1.List.Add(4);

            mOrmManager.SaveObject(o1);

            ObjectWithDerivedList o1_1 = mOrmManager.LoadObject<ObjectWithDerivedList>(o1.Id);

            Assert.That(o1_1.List, Has.Count.EqualTo(o1.List.Count), "o1_1.List.Count");

            for (int i = 0; i < o1.List.Count; ++i)
            {
                Assert.That(o1_1.List[i], Is.EqualTo(o1.List[i]), "obj1.Points[" + i.ToString() + "]");
            }

        }

        [Test(Description = "Person - Save and Find")]
        public void SaveFind()
        {
            PersonA jozo = new PersonA("Jozko mrkvicka", "Bratislava", new Account("JM", "JM123"), "text from A", "text from B");
            PersonA jozo2 = new PersonA("Jozko mrkvicka2", "Kosice", new Account("JM2", "JM123"), "text from A", "text from B");
            PersonA jozo3 = new PersonA("Jozko mrkvicka3", "Bratislava", new Account("JM3", "JM123"), "text from A", "text from B");

            long id = mOrmManager.SaveObject(jozo);
            long id2 = mOrmManager.SaveObject(jozo2);
            long id3 = mOrmManager.SaveObject(jozo3);

            long findId = mOrmManager.FindObject(typeof(PersonA), "Name ='Jozko mrkvicka'");
            long[] findIds = mOrmManager.FindObjects(typeof(PersonA), "Address ='Bratislava'");

            Assert.That(findId, Is.EqualTo(id), "Find Name ='Jozko mrkvicka'");
            Assert.That(findIds.Length, Is.EqualTo(2), "findIds.Count");
            Assert.That(findIds, Has.Some.EqualTo(id), "findIds contains id1");
            Assert.That(findIds, Has.Some.EqualTo(id3), "findIds contains id3");
        }

        [Test(Description = "Recovery from exception during save")]
        public void RepairObjectId()
        {
            PersonA jozo = new PersonA("Jozko mrkvicka2", "Niekde v BA", new Account("JM2", "JM123"), "text from A", "text from B");
            jozo.EnumTest = PersonA.TestEnum.test2;
            jozo.Childs.Add(new Person("Ferko mrkvicka", "Niekde v BA", new Account("FM", "FM123")));
            jozo.Childs.Add(new Person("Anca mrkvickova", "Niekde v BA", new Account("AM", "AM123")));
            mOrmManager.SaveObject(jozo);

            var zuza = new Person("Zuza mrkvickova", "Niekde v BA", new Account("AM", "AM123"));
            jozo.Childs.Add(zuza);

            bool thrown = false;
            try
            {
                mOrmManager.SaveObject(jozo);
            }
            catch
            {
                thrown = true;
            }
            Assert.That(thrown, Is.True, "SaveObject has not thrown");

            mOrmManager.RepairObjectId(jozo);
            zuza.Account.Name = "NiecoIne";

            mOrmManager.SaveObject(jozo);
        }

        [Test(Description = "SaveAs")]
        public void SaveAs()
        {
            PersonA jozo = new PersonA("A", "Niekde v BA", null, "text from A", "text from B");
            jozo.EnumTest = PersonA.TestEnum.test2;
            jozo.Childs.Add(new Person("B", "Niekde v BA", null));
            jozo.Childs.Add(new Person("C", "Niekde v BA", null));
            mOrmManager.SaveObject(jozo);

            mOrmManager.ClearObjectId(jozo);

            jozo.Name = "A2";
            jozo.Childs[0].Name = "B2";
            jozo.Childs[1].Name = "C2";

            mOrmManager.SaveObject(jozo);

            Person[] persons = mOrmManager.LoadObjects<Person>("Name = 'A' OR Name = 'B' OR Name = 'C' OR Name = 'A2' OR Name = 'B2' OR Name = 'C2'");
            Assert.That(persons.Length, Is.EqualTo(6), "persons.Count");
        }

        private void CheckPersonEquality(Person a, Person b)
        {
            Console.WriteLine("Checking equality for person " + a.Name);

            Assert.That(b.Id, Is.EqualTo(a.Id), "Person Id");
            Assert.That(b.Name, Is.EqualTo(a.Name), "Person Name");
            Assert.That(b.Address, Is.EqualTo(a.Address), "Person Address");
            Assert.That(b.Account.Name, Is.EqualTo(a.Account.Name), "Person Account.Name");
            Assert.That(b.Account.Password, Is.EqualTo(a.Account.Password), "Person Account.Password");

            //childrens
            Assert.That(b.Childs.Count, Is.EqualTo(a.Childs.Count), "Person Childrens.Count");
            Console.WriteLine("Checking childrens equality of " + a.Name);
            for (int i = 0; i < a.Childs.Count; ++i)
            {
                CheckPersonEquality(a.Childs[i], b.Childs[i]);
            }
            Console.WriteLine("Children check done for " + a.Name);
        }

        private void CheckPersonEquality(PersonA a, PersonA b)
        {
            Console.WriteLine("Checking equality for person (A) " + a.Name);
            CheckPersonEquality((Person)a, (Person)b);

            Assert.That(b.TextFromA, Is.EqualTo(PersonA.AfterLoadText), "Person TextFromA (text from AfterLoad)");
            Assert.That(b.TextFromB, Is.EqualTo(a.TextFromB), "Person TextFromB");
            Assert.That(b.EnumTest, Is.EqualTo(a.EnumTest), "Person EnumTest");
        }

        private Dictionary<long, IDataObject> GetCachedObjects(OrmManager ormManager)
        {
            return (Dictionary<long, IDataObject>)typeof(OrmManager).GetField("mCachedObjects", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ormManager);
        }

        private long GetRowCount(string tableName)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=file.db;Version=3;New=False;Compress=False"))
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM [" + tableName + "]";

                    connection.Open();
                    long rowCnt = (long)command.ExecuteScalar();
                    connection.Close();
                    return rowCnt;
                }
            }
        }
    }
}
