using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Should;

namespace HidLibrary.Tests
{
    public class HidEnumeratorTests
    {
        private HidEnumerator _enumerator;
        private string _devicePath;

        public void BeforeEach()
        {
            _enumerator = new HidEnumerator();
            var firstDevice = _enumerator.Enumerate().FirstOrDefault();

            _devicePath = firstDevice != null ? firstDevice.DevicePath : "";
        }

        [Fact]
        public void CanConstruct()
        {
            BeforeEach();
            _enumerator.ShouldBeType(typeof(HidEnumerator));
        }

        [Fact]
        public void WrapsIsConnected()
        {
            BeforeEach();
            var enumIsConnected = _enumerator.IsConnected(_devicePath);
            var hidIsConnected = HidDevices.IsConnected(_devicePath);
            enumIsConnected.ShouldEqual(hidIsConnected);
        }

        [Fact]
        public void WrapsGetDevice()
        {
            BeforeEach();
            var enumDevice = _enumerator.GetDevice(_devicePath);
            IHidDevice hidDevice = HidDevices.GetDevice(_devicePath);
            enumDevice.DevicePath.ShouldEqual(hidDevice.DevicePath);
        }

        [Fact]
        public void WrapsEnumerateDefault()
        {
            BeforeEach();
            var enumDevices = _enumerator.Enumerate();
            var hidDevices = HidDevices.Enumerate().
                Select(d => d as IHidDevice);

            
            AllDevicesTheSame(enumDevices, hidDevices).ShouldBeTrue();
        }

        [Fact]
        public void WrapsEnumerateDevicePath()
        {
            BeforeEach();
            var enumDevices =
                _enumerator.Enumerate(_devicePath);
            var hidDevices =
                HidDevices.Enumerate(_devicePath).
                    Select(d => d as IHidDevice);


            AllDevicesTheSame(enumDevices, hidDevices).ShouldBeTrue();
        }

        [Fact]
        public void WrapsEnumerateVendorId()
        {
            BeforeEach();
            var vid = GetVid();

            var enumDevices =
                _enumerator.Enumerate(vid);
            var hidDevices =
                HidDevices.Enumerate(vid).
                    Select(d => d as IHidDevice);


            AllDevicesTheSame(enumDevices, hidDevices).ShouldBeTrue();
        }

        [Fact]
        public void WrapsEnumerateVendorIdProductId()
        {
            BeforeEach();
            var vid = GetVid();
            var pid = GetPid();

            var enumDevices =
                _enumerator.Enumerate(vid, pid);
            var hidDevices =
                HidDevices.Enumerate(vid, pid).
                    Select(d => d as IHidDevice);


            AllDevicesTheSame(enumDevices, hidDevices).ShouldBeTrue();
        }

        private bool AllDevicesTheSame(IEnumerable<IHidDevice> a,
            IEnumerable<IHidDevice> b)
        {
            if(a.Count() != b.Count())
                return false;
            
            var allSame = true;

            var aList = a.ToList();
            var bList = b.ToList();

            var numDevices = aList.Count;

            for (var i = 0; i < numDevices; i++)
            {
                if (aList[i].DevicePath !=
                    bList[i].DevicePath)
                {
                    allSame = false;
                    break;
                }
            }

            return allSame;
        }

        private int GetVid()
        {
            return GetNumberFromRegex("vid_([0-9a-f]{4})");
        }

        private int GetPid()
        {
            return GetNumberFromRegex("pid_([0-9a-f]{3,4})");
        }

        private int GetNumberFromRegex(string pattern)
        {
            var match = Regex.Match(_devicePath, pattern,
                RegexOptions.IgnoreCase);

            var num = 0;

            if (match.Success)
            {
                num = int.Parse(match.Groups[1].Value,
                    System.Globalization.NumberStyles.HexNumber);
            }

            return num;
        }
    }
}
