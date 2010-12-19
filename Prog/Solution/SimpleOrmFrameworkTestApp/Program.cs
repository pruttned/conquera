using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOrmFramework;
using System.IO;
using SimpleOrmFrameworkTest;
using System.Drawing;

namespace SimpleOrmFrameworkTestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            if (File.Exists("file.db"))
            {
                File.Delete("file.db");
            }

            using (OrmManager mOrmManager = new OrmManager("Data Source=file.db;Version=3;New=False;Compress=False"))
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


                //Person person = new Person("ahoj", "cau", null);
                //Person person2 = new Person("ahoj", "cau2", null);
                //Person person3 = new Person("ahodj", "cau2", null);

                //using (SofTransaction transaction = mOrmManager.BeginTransaction())
                //{
                //    try
                //    {
                //        mOrmManager.SaveObject(person);
                //        mOrmManager.SaveObject(person2);

                //        transaction.Commit();

                //    }
                //    catch
                //    {
                //        var a = mOrmManager.ExistsObjectWithId(2);
                //    }
                //}
                //mOrmManager.RepairObjectId(person);
                //mOrmManager.RepairObjectId(person2);

                //using (SofTransaction transaction = mOrmManager.BeginTransaction())
                //{
                //    mOrmManager.SaveObject(person3);
                //    transaction.Commit();
            
                //}
                //using (SofTransaction transaction = mOrmManager.BeginTransaction())
                //{
                //    int[] list = new int[] { 10, 20, 30 };
                //    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                //    {
                //        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                //        f.Serialize(ms, list);
                //        mOrmManager.SetBlobData("Test", ms.ToArray());
                //    }

                //    transaction.Commit();
                //}

                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream(mOrmManager.GetBlob("Test")))
                //{
                //    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                //    var list = f.Deserialize(ms);
                //}

                //bool del = mOrmManager.DeleteBlobData("Test");

                //ObjectWithDerivedList o1 = new ObjectWithDerivedList();
                //o1.List.Add(1);
                //o1.List.Add(2);
                //o1.List.Add(3);

                //mOrmManager.SaveObject(o1);

                //o1.List.Add(4);

                //mOrmManager.SaveObject(o1);

                //ObjectWithDerivedList o1_1 = mOrmManager.LoadObject<ObjectWithDerivedList>(o1.Id);


            }
        }
    }
}
