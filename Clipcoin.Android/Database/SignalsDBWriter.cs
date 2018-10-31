using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clipcoin.Phone.Database.Sqlite;
using Clipcoin.Phone.Help;
using Trigger.Beacons;
using Trigger.Classes.Beacons;

namespace Clipcoin.Phone.Database
{
    public class SignalsDBWriter
    {
        public const string MacAddressKey = "macaddress";
        public const string RssiKey = "rssi";
        public const string APointKey = "apoint";
        public const string DateTimeKey = "datetime";



        public const string BeaconTableName = "beacon_signals";
        private Context context;
        public DBHelper Helper { get; private set; }
        public IDictionary<string, string> Headers
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {MacAddressKey, "text" },
                    {RssiKey, "int" },
                    {APointKey, "text" },
                    {DateTimeKey, "text" }
                };
            }
        }

        public SignalsDBWriter(Context c)
        {
            context = c ?? throw new NullReferenceException("Context cannot be null.");
            Helper = new DBHelper(c);
        }

        public void ClearData()
        {
            if (Helper.TableExist(BeaconTableName))
            {
                Helper.ClearTable(BeaconTableName);
            }
        }

        public void NewBeaconSignal(Beacon beacon, string apointUid, DateTime time)
        {
            if (!Helper.TableExist(BeaconTableName))
            {
                Helper.CreateNewTable(BeaconTableName, Headers);
            }

            var cv = new ContentValues();

            cv.Put(MacAddressKey, beacon.BluetoothAddress);
            cv.Put(RssiKey, beacon.Rssi);
            cv.Put(APointKey, apointUid);
            cv.Put(DateTimeKey, Tools.FormateDateTimeToTime(time));

            try
            {
                long rowIndex = Helper.WritableDatabase.Insert(BeaconTableName, null, cv);
            }
            catch (Exception exp)
            {

            }
        }

        public void AddNewBeaconTelemetry(ICollection<Beacon> beacons, string apointUid, DateTime time)
        {
            if (!Helper.TableExist(BeaconTableName))
            {
                Helper.CreateNewTable(BeaconTableName, Headers);
            }

            foreach (var b in beacons)
            {

                var cv = new ContentValues();

                cv.Put(MacAddressKey, b.BluetoothAddress);
                cv.Put(RssiKey, b.Rssi);
                cv.Put(DateTimeKey, Tools.FormateDateTimeToTime(time));

                try
                {
                    long rowIndex = Helper.WritableDatabase.Insert(BeaconTableName, null, cv);
                }
                catch (Exception exp)
                {

                }

            }
        }

        public void DeleteById(int id)
        {
            Helper.DeleteId(BeaconTableName, id.ToString());
        }



        public Trigger.Signal.Telemetry ReadTelemetry(string userId, out ICollection<int> ids)
        {
            ids = new List<int>();

            Trigger.Signal.Telemetry telemetry = Trigger.Signal.Telemetry.EmptyForUser(userId);

            ICursor cursor = Helper.ReadableDatabase.Query(BeaconTableName, null, null, null, null, null, null);

            if (cursor.MoveToFirst())
            {
                int idColIndex = cursor.GetColumnIndex("id");
                int macAddressColIndex = cursor.GetColumnIndex(MacAddressKey);
                int rssiColIndex = cursor.GetColumnIndex(RssiKey);
                int dateColIndex = cursor.GetColumnIndex(DateTimeKey);
                int uidColIndex = cursor.GetColumnIndex(APointKey);

                int id = cursor.GetInt(idColIndex);
                string mac = cursor.GetString(macAddressColIndex);
                int rssi = cursor.GetInt(rssiColIndex);
                string uid = cursor.GetString(uidColIndex);
                DateTime time = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>($"\"{cursor.GetString(dateColIndex)}\"");

                telemetry.Append(
                    new BeaconData
                    {
                        Address = mac
                    }.Add(new BeaconItem[]
                    {
                         new BeaconItem
                        {
                            Rssi = rssi,
                            Time = time
                        }
                    }));

                ids.Add(id);

                while (cursor.MoveToNext())
                {
                    id = cursor.GetInt(idColIndex);
                    mac = cursor.GetString(macAddressColIndex);
                    rssi = cursor.GetInt(rssiColIndex);
                    uid = cursor.GetString(uidColIndex);
                    time = Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>($"\"{cursor.GetString(dateColIndex)}\"");

                    telemetry.Append(
                    new BeaconData
                    {
                        Address = mac
                    }.Add(new BeaconItem[]
                    {
                         new BeaconItem
                        {
                            Rssi = rssi,
                            Time = time
                        }
                    }));


                    ids.Add(id);
                }
            }

            return telemetry;
        }
    }
}