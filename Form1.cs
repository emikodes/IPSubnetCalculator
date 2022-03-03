
/*Copyright (c) 2015, lduchosal
All rights reserved.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPSubnetCalculator
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelMask.Visible = false;
            textBoxMaskBits.Visible = false;
            plusMaskBits.Visible = false;
            minusMaskBits.Visible = false;
            sliderSubnetNumber.Visible = false;
            labelSliderValue.Visible=false;
            

            textBoxIPAddress.PlaceholderText = "Enter Default IP Address (CIDR)";
            
            textBoxMaskBits.TextAlign = HorizontalAlignment.Center;
            textBoxMaskBits.ReadOnly = true;
            textBoxMaskBits.Text = "Enter IP Address first.";

            sliderSubnetNumber.Maximum = 1;
            sliderSubnetNumber.Minimum = 0;
            sliderSubnetNumber.SmallChange = 1;
            sliderSubnetNumber.Value = 1;

            labelSliderValue.TextAlignment = ContentAlignment.MiddleCenter;
            labelSliderValue.Text = "1";

            textBoxSubnets.AcceptsReturn = true;
            textBoxSubnets.Multiline = true;

        }
        private void plusMaskBits_Click(object sender, EventArgs e)
        {
            int value;
            if (int.TryParse(textBoxMaskBits.Text, out value) == true && value < 30)
            {
                textBoxMaskBits.Text = Convert.ToString(++value);
            }
            textBoxSubnetMask.Text = Convert.ToString((value), 2);

        }

        private void minusMaskBits_Click(object sender, EventArgs e)
        {
            int value;
            if (int.TryParse(textBoxMaskBits.Text, out value) == true && value > 1 && value>Convert.ToInt32(textBoxIPAddress.Text.Split('/')[1]))
            {
                textBoxMaskBits.Text = Convert.ToString(--value);
            }
            textBoxSubnetMask.Text = Convert.ToString((value), 2);

        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            try
            {
                byte cidr = 0;
                string mysubnetmask = string.Empty;

                textBoxSubnetMask.Text = String.Empty;
                textBoxSubnets.Text = String.Empty;

                IPNetwork net = IPNetwork.Parse(textBoxIPAddress.Text);

                if (toggleSwitchMaskOrNetNumber.Checked == true) //textbox with plus and minus
                {
                    cidr = Convert.ToByte(textBoxMaskBits.Text);
                }
                else
                {
                    //convert slidervalue (subnet number) TO cidr (log...)
                    cidr = Convert.ToByte(Convert.ToInt32(textBoxIPAddress.Text.Split('/')[1]) + Convert.ToInt32(Convert.ToString(sliderSubnetNumber.Value,2).Length));
                }

                IPNetworkCollection subnets = net.Subnet(cidr);

                string[] ottetti = Convert.ToString(subnets[0].Netmask).Split('.');
                

                labelSubnets.Text = "Subnets: (" + Convert.ToString(subnets.Count) + ")";


                foreach (string group in ottetti)
                {
                    textBoxSubnetMask.Text += Convert.ToString(Convert.ToInt64(group), 2).PadLeft(8, '0') + " ";
                }

                foreach (IPNetwork singleNet in subnets)
                {
                    textBoxSubnets.AppendText("Network: " + Convert.ToString(singleNet.Network) + " First IP: " + Convert.ToString(singleNet.FirstUsable) + " Last IP: " + Convert.ToString(singleNet.LastUsable) + " Total Hosts: " + Convert.ToString(singleNet.Total) + " Usable: " + Convert.ToString(singleNet.Total == 1 ? 0 : singleNet.Total - 2) +" Broadcast: " + Convert.ToString(singleNet.Broadcast) + Environment.NewLine);
                }
            }
            catch
            {
                MessageBox.Show("Ricontrollare dati inseriti in input.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sliderSubnetNumber_ValueChanged(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ValueChangedEventArgs e)
        {
            labelSliderValue.Text = Convert.ToString(sliderSubnetNumber.Value);
        }

        private void textBoxIPAddress_TextChange(object sender, EventArgs e)
        {
            try
            {
                string[] parsedIP;

                if ((parsedIP = textBoxIPAddress.Text.Split('/')).Length == 2 && Enumerable.Range(1, 32).Contains(Int32.Parse(parsedIP[1])))
                {
                    textBoxMaskBits.Text = parsedIP[1];
                    sliderSubnetNumber.Maximum = (int)(Math.Pow(2, (30 - Convert.ToInt32(parsedIP[1]))))-1; //MAX BITS MASK = 30 (NOT 32)
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                textBoxMaskBits.Text = "IP Address not valid.";
            }

        }

        private T reinterpret_cast<T>(double v)
        {
            throw new NotImplementedException();
        }

        private void toggleSwitchMaskOrNetNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (toggleSwitchMaskOrNetNumber.Checked == true)
            {
                labelMaskOrNetNumber.Text = "Netmask (CIDR):";
                sliderSubnetNumber.Visible = false;
                labelSliderValue.Visible = false;

                labelMask.Visible = true;
                textBoxMaskBits.Visible = true;
                plusMaskBits.Visible = true;
                minusMaskBits.Visible = true;
            }
            else
            {
                labelMaskOrNetNumber.Text = "Net Number:";
                labelMask.Visible = false;
                textBoxMaskBits.Visible = false;
                plusMaskBits.Visible = false;
                minusMaskBits.Visible = false;

                labelSliderValue.Visible = true;
                sliderSubnetNumber.Visible = true;
            }

        }

        private void textBoxSubnets_Click(object sender, EventArgs e)
        {
            if (textBoxSubnets.Text != "")
            {
                Clipboard.SetText(textBoxSubnets.Text);
                MessageBox.Show("The textbox content has been copied in the clipboard.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public class IPNetworkCollection : IEnumerable<IPNetwork>, IEnumerator<IPNetwork>
    {

        private BigInteger _enumerator;
        private byte _cidrSubnet;
        private IPNetwork _ipnetwork;

        private byte _cidr
        {
            get { return this._ipnetwork.Cidr; }
        }
        private BigInteger _broadcast
        {
            get { return IPNetwork.ToBigInteger(this._ipnetwork.Broadcast); }
        }
        private BigInteger _lastUsable
        {
            get { return IPNetwork.ToBigInteger(this._ipnetwork.LastUsable); }
        }
        private BigInteger _network
        {
            get { return IPNetwork.ToBigInteger(this._ipnetwork.Network); }
        }

#if TRAVISCI
        public
#else
        internal
#endif
        IPNetworkCollection(IPNetwork ipnetwork, byte cidrSubnet)
        {

            int maxCidr = ipnetwork.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;
            if (cidrSubnet > maxCidr)
            {
                throw new ArgumentOutOfRangeException("cidrSubnet");
            }

            if (cidrSubnet < ipnetwork.Cidr)
            {
                throw new ArgumentException("cidr");
            }

            this._cidrSubnet = cidrSubnet;
            this._ipnetwork = ipnetwork;
            this._enumerator = -1;
        }

        public BigInteger Count
        {
            get
            {
                BigInteger count = BigInteger.Pow(2, this._cidrSubnet - this._cidr);
                return count;
            }
        }

        public IPNetwork this[BigInteger i]
        {
            get
            {
                if (i >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("i");
                }

                BigInteger last = this._ipnetwork.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                    ? this._lastUsable : this._broadcast;
                BigInteger increment = (last - this._network) / this.Count;
                BigInteger uintNetwork = this._network + ((increment + 1) * i);
                IPNetwork ipn = new IPNetwork(uintNetwork, this._ipnetwork.AddressFamily, this._cidrSubnet);
                return ipn;
            }
        }


        IEnumerator<IPNetwork> IEnumerable<IPNetwork>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }


        public IPNetwork Current
        {
            get { return this[this._enumerator]; }
        }


        public void Dispose()
        {
            // nothing to dispose
            return;
        }


        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            this._enumerator++;
            if (this._enumerator >= this.Count)
            {
                return false;
            }
            return true;

        }

        public void Reset()
        {
            this._enumerator = -1;
        }


    }

    public class IPAddressCollection : IEnumerable<IPAddress>, IEnumerator<IPAddress>
    {

        private readonly IPNetwork _ipnetwork;
        private readonly FilterEnum _filter;
        private BigInteger _enumerator;

        internal IPAddressCollection(IPNetwork ipnetwork, FilterEnum filter)
        {
            this._ipnetwork = ipnetwork;
            this._filter = filter;
            Reset();
        }


        public BigInteger Count
        {
            get
            {

                BigInteger count = _ipnetwork.Total;
                if (this._filter == FilterEnum.Usable)
                {
                    count -= 2;
                }
                if (count < 0)
                {
                    count = 0;
                }

                return count;
            }
        }

        public IPAddress this[BigInteger i]
        {
            get
            {
                if (i >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("i");
                }
                byte width = this._ipnetwork.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? (byte)32 : (byte)128;
                IPNetworkCollection ipn = this._ipnetwork.Subnet(width);

                BigInteger index = i;
                if (this._filter == FilterEnum.Usable)
                {
                    index++;
                }
                return ipn[index].Network;
            }
        }


        public IPAddress Current
        {
            get
            {
                return this[_enumerator];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            _enumerator++;
            if (_enumerator >= this.Count)
            {
                return false;
            }
            return true;
        }

        public void Reset()
        {
            _enumerator = -1;
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        IEnumerator<IPAddress> IEnumerable<IPAddress>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<IPAddress>
        {
            private readonly IPAddressCollection _collection;
            private BigInteger _enumerator;

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public IPAddress Current
            {
                get
                {
                    return _collection[_enumerator];
                }
            }


            public void Dispose()
            {
                // nothing to dispose
            }

            public bool MoveNext()
            {
                _enumerator++;
                if (_enumerator >= _collection.Count)
                {
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                _enumerator = -1;
            }

            public Enumerator(IPAddressCollection collection)
            {
                _collection = collection;
                _enumerator = -1;
            }
        }
    }

    public sealed class IPNetwork : IComparable<IPNetwork>, ISerializable
    {


        //private uint _network;
        private BigInteger _ipaddress;
        private AddressFamily _family;
        
        private byte _cidr;

        public string Value
        {
            get { return this.ToString(); }
            set
            {
                var ipnetwork = IPNetwork.Parse(value);
                this._ipaddress = ipnetwork._ipaddress;
                this._family = ipnetwork._family;
                this._cidr = ipnetwork._cidr;
            }
        }


        private BigInteger _network
        {
            get
            {
                BigInteger uintNetwork = this._ipaddress & this._netmask;
                return uintNetwork;
            }
        }

        /// <summary>
        /// Network address
        /// </summary>
        public IPAddress Network
        {
            get
            {

                return IPNetwork.ToIPAddress(this._network, this._family);
            }
        }

        /// <summary>
        /// Address Family
        /// </summary>
        public AddressFamily AddressFamily
        {
            get
            {
                return this._family;
            }
        }

        private BigInteger _netmask
        {
            get
            {
                return IPNetwork.ToUint(this._cidr, this._family);
            }
        }

        /// <summary>
        /// Netmask
        /// </summary>
        public IPAddress Netmask
        {
            get
            {
                return IPNetwork.ToIPAddress(this._netmask, this._family);
            }
        }

        private BigInteger _broadcast
        {
            get
            {

                int width = this._family == System.Net.Sockets.AddressFamily.InterNetwork ? 4 : 16;
                BigInteger uintBroadcast = this._network + this._netmask.PositiveReverse(width);
                return uintBroadcast;
            }
        }

        static BigInteger CreateBroadcast(ref BigInteger network, BigInteger netmask, AddressFamily family)
        {
            int width = family == AddressFamily.InterNetwork ? 4 : 16;
            BigInteger uintBroadcast = network + netmask.PositiveReverse(width);

            return uintBroadcast;
        }

        /// <summary>
        /// Broadcast address
        /// </summary>
        public IPAddress Broadcast
        {
            get
            {
                if (this._family == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return null;
                }
                return IPNetwork.ToIPAddress(this._broadcast, this._family);
            }
        }

    
        public IPAddress FirstUsable
        {
            get
            {
                BigInteger fisrt = this._family == System.Net.Sockets.AddressFamily.InterNetworkV6 ? this._network : (this.Usable <= 0) ? this._network : this._network + 1;
                return IPNetwork.ToIPAddress(fisrt, this._family);
            }
        }

 
        public IPAddress LastUsable
        {
            get
            {
                BigInteger last = this._family == System.Net.Sockets.AddressFamily.InterNetworkV6 ? this._broadcast : (this.Usable <= 0) ? this._network : this._broadcast - 1;
                return IPNetwork.ToIPAddress(last, this._family);
            }
        }

        public BigInteger Usable
        {
            get
            {

                if (this._family == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return this.Total;
                }
                byte[] mask = new byte[] { 0xff, 0xff, 0xff, 0xff, 0x00 };
                BigInteger bmask = new BigInteger(mask);
                BigInteger usableIps = (_cidr > 30) ? 0 : ((bmask >> _cidr) - 1);
                return usableIps;
            }
        }

        public BigInteger Total
        {
            get
            {

                int max = this._family == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;
                BigInteger count = BigInteger.Pow(2, (max - _cidr));
                return count;
            }
        }


        public byte Cidr
        {
            get
            {
                return this._cidr;
            }
        }

#if TRAVISCI
        public
#else
        internal
#endif
            IPNetwork(BigInteger ipaddress, AddressFamily family, byte cidr)
        {
            Init(ipaddress, family, cidr);
        }

        public IPNetwork(IPAddress ipaddress, byte cidr)
        {
            if (ipaddress == null) throw new ArgumentNullException(nameof(ipaddress));

            var uintIpAddress = ToBigInteger(ipaddress);

            Init(uintIpAddress, ipaddress.AddressFamily, cidr);
        }

        private void Init(BigInteger ipaddress, AddressFamily family, byte cidr)
        {
            var maxCidr = family == AddressFamily.InterNetwork ? 32 : 128;
            if (cidr > maxCidr)
            {
                throw new ArgumentOutOfRangeException("cidr");
            }

            _ipaddress = ipaddress;
            _family = family;
            _cidr = cidr;
        }

        public static IPNetwork Parse(string ipaddress, string netmask)
        {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, ipaddress, netmask, out ipnetwork);
            return ipnetwork;
        }
        public static IPNetwork Parse(string ipaddress, byte cidr)
        {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, ipaddress, cidr, out ipnetwork);
            return ipnetwork;

        }

        public static IPNetwork Parse(IPAddress ipaddress, IPAddress netmask)
        {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, ipaddress, netmask, out ipnetwork);
            return ipnetwork;

        }

        public static IPNetwork Parse(string network)
        {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, network, CidrGuess.ClassFull, out ipnetwork);
            return ipnetwork;

        }


        public static IPNetwork Parse(string network, ICidrGuess cidrGuess)
        {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, network, cidrGuess, out ipnetwork);
            return ipnetwork;

        }

        private static void InternalParse(bool tryParse, string ipaddress, string netmask, out IPNetwork ipnetwork)
        {

            if (string.IsNullOrEmpty(ipaddress))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            if (string.IsNullOrEmpty(netmask))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("netmask");
                }
                ipnetwork = null;
                return;
            }

            IPAddress ip = null;
            bool ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);
            if (ipaddressParsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress mask = null;
            bool netmaskParsed = IPAddress.TryParse(netmask, out mask);
            if (netmaskParsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("netmask");
                }
                ipnetwork = null;
                return;
            }

            IPNetwork.InternalParse(tryParse, ip, mask, out ipnetwork);
        }

        private static void InternalParse(bool tryParse, string network, ICidrGuess cidrGuess, out IPNetwork ipnetwork)
        {

            if (string.IsNullOrEmpty(network))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("network");
                }
                ipnetwork = null;
                return;
            }

            network = network.Replace(@"[^0-9a-fA-F\.\/\s\:]+", "");
            network = network.Replace(@"\s{2,}", " ");
            network = network.Trim();
            string[] args = network.Split(new char[] { ' ', '/' });
            byte cidr = 0;
            if (args.Length == 1)
            {

                string cidrlessNetwork = args[0];
                if (cidrGuess.TryGuessCidr(cidrlessNetwork, out cidr))
                {
                    IPNetwork.InternalParse(tryParse, cidrlessNetwork, cidr, out ipnetwork);
                    return;
                }

                if (tryParse == false)
                {
                    throw new ArgumentException("network");
                }
                ipnetwork = null;
                return;
            }

            if (byte.TryParse(args[1], out cidr))
            {
                IPNetwork.InternalParse(tryParse, args[0], cidr, out ipnetwork);
                return;
            }

            IPNetwork.InternalParse(tryParse, args[0], args[1], out ipnetwork);
            return;

        }


        private static void InternalParse(bool tryParse, IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork)
        {

            if (ipaddress == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            if (netmask == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("netmask");
                }
                ipnetwork = null;
                return;
            }

            BigInteger uintIpAddress = IPNetwork.ToBigInteger(ipaddress);
            byte? cidr2;
            bool parsed = IPNetwork.TryToCidr(netmask, out cidr2);
            if (parsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("netmask");
                }
                ipnetwork = null;
                return;
            }
            byte cidr = (byte)cidr2;

            IPNetwork ipnet = new IPNetwork(uintIpAddress, ipaddress.AddressFamily, cidr);
            ipnetwork = ipnet;

            return;
        }



        private static void InternalParse(bool tryParse, string ipaddress, byte cidr, out IPNetwork ipnetwork)
        {

            if (string.IsNullOrEmpty(ipaddress))
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }


            IPAddress ip;
            bool ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);
            if (ipaddressParsed == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress mask = null;
            bool parsedNetmask = IPNetwork.TryToNetmask(cidr, ip.AddressFamily, out mask);
            if (parsedNetmask == false)
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("cidr");
                }
                ipnetwork = null;
                return;
            }


            IPNetwork.InternalParse(tryParse, ip, mask, out ipnetwork);
        }
        public static BigInteger ToBigInteger(IPAddress ipaddress)
        {
            BigInteger? uintIpAddress = null;
            IPNetwork.InternalToBigInteger(false, ipaddress, out uintIpAddress);
            return (BigInteger)uintIpAddress;

        }

        public static bool TryToBigInteger(IPAddress ipaddress, out BigInteger? uintIpAddress)
        {
            BigInteger? uintIpAddress2 = null;
            IPNetwork.InternalToBigInteger(true, ipaddress, out uintIpAddress2);
            bool parsed = (uintIpAddress2 != null);
            uintIpAddress = uintIpAddress2;
            return parsed;
        }

#if TRAVISCI
        public
#else
        internal
#endif
            static void InternalToBigInteger(bool tryParse, IPAddress ipaddress, out BigInteger? uintIpAddress)
        {

            if (ipaddress == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("ipaddress");
                }
                uintIpAddress = null;
                return;
            }

#if NET5_0 || NETSTANDARD2_1
            byte[] bytes = ipaddress.AddressFamily == AddressFamily.InterNetwork ? new byte[4] : new byte[16];
            var span = bytes.AsSpan();
            if (!ipaddress.TryWriteBytes(span, out _))
            {
                if (tryParse == false) {
                    throw new ArgumentException("ipaddress");
                }

                uintIpAddress = null;
                return;
            }

            uintIpAddress = new BigInteger(span, isUnsigned: true, isBigEndian: true);
#elif NET45 || NET46 || NET47 || NETSTANDARD20
            byte[] bytes = ipaddress.GetAddressBytes();
            bytes.AsSpan().Reverse();

            // add trailing 0 to make unsigned
            var unsigned = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, unsigned, 0, bytes.Length);
            uintIpAddress = new BigInteger(unsigned);
#else
            byte[] bytes = ipaddress.GetAddressBytes();
            Array.Reverse(bytes);

            // add trailing 0 to make unsigned
            var unsigned = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, unsigned, 0, bytes.Length);
            uintIpAddress = new BigInteger(unsigned);
#endif
        }

        public static BigInteger ToUint(byte cidr, AddressFamily family)
        {

            BigInteger? uintNetmask = null;
            IPNetwork.InternalToBigInteger(false, cidr, family, out uintNetmask);
            return (BigInteger)uintNetmask;
        }

        public static bool TryToUint(byte cidr, AddressFamily family, out BigInteger? uintNetmask)
        {

            BigInteger? uintNetmask2 = null;
            IPNetwork.InternalToBigInteger(true, cidr, family, out uintNetmask2);
            bool parsed = (uintNetmask2 != null);
            uintNetmask = uintNetmask2;
            return parsed;
        }

#if TRAVISCI
        public
#else
        internal
#endif
        static void InternalToBigInteger(bool tryParse, byte cidr, AddressFamily family, out BigInteger? uintNetmask)
        {

            if (family == AddressFamily.InterNetwork && cidr > 32)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                uintNetmask = null;
                return;
            }

            if (family == AddressFamily.InterNetworkV6 && cidr > 128)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                uintNetmask = null;
                return;
            }

            if (family != AddressFamily.InterNetwork
                && family != AddressFamily.InterNetworkV6)
            {
                if (tryParse == false)
                {
                    throw new NotSupportedException(family.ToString());
                }
                uintNetmask = null;
                return;
            }

            if (family == AddressFamily.InterNetwork)
            {

                uintNetmask = cidr == 0 ? 0 : 0xffffffff << (32 - cidr);
                return;
            }

            BigInteger mask = new BigInteger(new byte[] {
                0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff,
                0x00
            });

            BigInteger masked = cidr == 0 ? 0 : mask << (128 - cidr);
            byte[] m = masked.ToByteArray();
            byte[] bmask = new byte[17];
            int copy = m.Length > 16 ? 16 : m.Length;
            Array.Copy(m, 0, bmask, 0, copy);
            uintNetmask = new BigInteger(bmask);


        }
        private static void InternalToCidr(bool tryParse, BigInteger netmask, AddressFamily family, out byte? cidr)
        {

            if (!IPNetwork.InternalValidNetmask(netmask, family))
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("netmask");
                }
                cidr = null;
                return;
            }

            byte cidr2 = IPNetwork.BitsSet(netmask, family);
            cidr = cidr2;
            return;

        }

        public static byte ToCidr(IPAddress netmask)
        {
            byte? cidr = null;
            IPNetwork.InternalToCidr(false, netmask, out cidr);
            return (byte)cidr;
        }

        public static bool TryToCidr(IPAddress netmask, out byte? cidr)
        {
            byte? cidr2 = null;
            IPNetwork.InternalToCidr(true, netmask, out cidr2);
            bool parsed = (cidr2 != null);
            cidr = cidr2;
            return parsed;
        }

        private static void InternalToCidr(bool tryParse, IPAddress netmask, out byte? cidr)
        {

            if (netmask == null)
            {
                if (tryParse == false)
                {
                    throw new ArgumentNullException("netmask");
                }
                cidr = null;
                return;
            }
            BigInteger? uintNetmask2 = null;
            bool parsed = IPNetwork.TryToBigInteger(netmask, out uintNetmask2);

            BigInteger uintNetmask = (BigInteger)uintNetmask2;

            byte? cidr2 = null;
            IPNetwork.InternalToCidr(tryParse, uintNetmask, netmask.AddressFamily, out cidr2);
            cidr = cidr2;

            return;

        }



        public static IPAddress ToNetmask(byte cidr, AddressFamily family)
        {

            IPAddress netmask = null;
            IPNetwork.InternalToNetmask(false, cidr, family, out netmask);
            return netmask;
        }

        public static bool TryToNetmask(byte cidr, AddressFamily family, out IPAddress netmask)
        {

            IPAddress netmask2 = null;
            IPNetwork.InternalToNetmask(true, cidr, family, out netmask2);
            bool parsed = (netmask2 != null);
            netmask = netmask2;
            return parsed;
        }


#if TRAVISCI
        public
#else
        internal
#endif
            static void InternalToNetmask(bool tryParse, byte cidr, AddressFamily family, out IPAddress netmask)
        {

            if (family != AddressFamily.InterNetwork
                && family != AddressFamily.InterNetworkV6)
            {
                if (tryParse == false)
                {
                    throw new ArgumentException("family");
                }
                netmask = null;
                return;
            }


            int maxCidr = family == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;
            if (cidr > maxCidr)
            {
                if (tryParse == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                netmask = null;
                return;
            }

            BigInteger mask = IPNetwork.ToUint(cidr, family);
            IPAddress netmask2 = IPNetwork.ToIPAddress(mask, family);
            netmask = netmask2;

            return;
        }

        private static byte BitsSet(BigInteger netmask, AddressFamily family)
        {

            string s = netmask.ToBinaryString();
            return (byte)s.Replace("0", "")
                .ToCharArray()
                .Length;

        }

        public static uint BitsSet(IPAddress netmask)
        {
            BigInteger uintNetmask = IPNetwork.ToBigInteger(netmask);
            uint bits = IPNetwork.BitsSet(uintNetmask, netmask.AddressFamily);
            return bits;
        }

        public static bool ValidNetmask(IPAddress netmask)
        {

            if (netmask == null)
            {
                throw new ArgumentNullException("netmask");
            }
            BigInteger uintNetmask = IPNetwork.ToBigInteger(netmask);
            bool valid = IPNetwork.InternalValidNetmask(uintNetmask, netmask.AddressFamily);
            return valid;
        }

#if TRAVISCI
        public
#else
        internal
#endif
            static bool InternalValidNetmask(BigInteger netmask, AddressFamily family)
        {

            if (family != AddressFamily.InterNetwork
                && family != AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException("family");
            }

            var mask = family == AddressFamily.InterNetwork
                ? new BigInteger(0x0ffffffff)
                : new BigInteger(new byte[]{
                    0xff, 0xff, 0xff, 0xff,
                    0xff, 0xff, 0xff, 0xff,
                    0xff, 0xff, 0xff, 0xff,
                    0xff, 0xff, 0xff, 0xff,
                    0x00
                });

            BigInteger neg = ((~netmask) & (mask));
            bool isNetmask = ((neg + 1) & neg) == 0;
            return isNetmask;

        }

        public static IPAddress ToIPAddress(BigInteger ipaddress, AddressFamily family)
        {

            int width = family == AddressFamily.InterNetwork ? 4 : 16;
            byte[] bytes = ipaddress.ToByteArray();
            byte[] bytes2 = new byte[width];
            int copy = bytes.Length > width ? width : bytes.Length;
            Array.Copy(bytes, 0, bytes2, 0, copy);
            Array.Reverse(bytes2);

            byte[] sized = Resize(bytes2, family);
            IPAddress ip = new IPAddress(sized);
            return ip;
        }

#if TRAVISCI
        public
#else
        internal
#endif
        static byte[] Resize(byte[] bytes, AddressFamily family)
        {

            if (family != AddressFamily.InterNetwork
                && family != AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException("family");
            }

            int width = family == AddressFamily.InterNetwork ? 4 : 16;

            if (bytes.Length > width)
            {
                throw new ArgumentException("bytes");
            }

            byte[] result = new byte[width];
            Array.Copy(bytes, 0, result, 0, bytes.Length);
            return result;
        }
        public bool Contains(IPAddress ipaddress)
        {

            if (ipaddress == null)
            {
                throw new ArgumentNullException("ipaddress");
            }

            if (AddressFamily != ipaddress.AddressFamily)
            {
                return false;
            }

            BigInteger uintNetwork = _network;
            BigInteger uintBroadcast = CreateBroadcast(ref uintNetwork, this._netmask, this._family);
            BigInteger uintAddress = IPNetwork.ToBigInteger(ipaddress);

            bool contains = (uintAddress >= uintNetwork
                && uintAddress <= uintBroadcast);

            return contains;
        }

        [Obsolete("static Contains is deprecated, please use instance Contains.")]

        public bool Contains(IPNetwork network2)
        {

            if (network2 == null)
            {
                throw new ArgumentNullException("network2");
            }

            BigInteger uintNetwork = _network;
            BigInteger uintBroadcast = CreateBroadcast(ref uintNetwork, this._netmask, this._family);

            BigInteger uintFirst = network2._network;
            BigInteger uintLast = CreateBroadcast(ref uintFirst, network2._netmask, network2._family);

            bool contains = (uintFirst >= uintNetwork
                && uintLast <= uintBroadcast);

            return contains;
        }

        

       

        private static readonly Lazy<IPNetwork> _iana_ablock_reserved = new Lazy<IPNetwork>(() => IPNetwork.Parse("10.0.0.0/8"));
        private static readonly Lazy<IPNetwork> _iana_bblock_reserved = new Lazy<IPNetwork>(() => IPNetwork.Parse("172.16.0.0/12"));
        private static readonly Lazy<IPNetwork> _iana_cblock_reserved = new Lazy<IPNetwork>(() => IPNetwork.Parse("192.168.0.0/16"));


        public static IPNetwork IANA_ABLK_RESERVED1
        {
            get
            {
                return _iana_ablock_reserved.Value;
            }
        }

        public static IPNetwork IANA_BBLK_RESERVED1
        {
            get
            {
                return _iana_bblock_reserved.Value;
            }
        }

        public static IPNetwork IANA_CBLK_RESERVED1
        {
            get
            {
                return _iana_cblock_reserved.Value;
            }
        }

        public static bool IsIANAReserved(IPAddress ipaddress)
        {

            if (ipaddress == null)
            {
                throw new ArgumentNullException("ipaddress");
            }

            return IPNetwork.IANA_ABLK_RESERVED1.Contains(ipaddress)
                || IPNetwork.IANA_BBLK_RESERVED1.Contains(ipaddress)
                || IPNetwork.IANA_CBLK_RESERVED1.Contains(ipaddress);
        }

        public bool IsIANAReserved()
        {
            return IPNetwork.IANA_ABLK_RESERVED1.Contains(this)
                || IPNetwork.IANA_BBLK_RESERVED1.Contains(this)
                || IPNetwork.IANA_CBLK_RESERVED1.Contains(this);
        }

        [Obsolete("static IsIANAReserved is deprecated, please use instance IsIANAReserved.")]
        public static bool IsIANAReserved(IPNetwork ipnetwork)
        {

            if (ipnetwork == null)
            {
                throw new ArgumentNullException("ipnetwork");
            }

            return ipnetwork.IsIANAReserved();
        }

        public IPNetworkCollection Subnet(byte cidr)
        {
            IPNetworkCollection ipnetworkCollection = null;
            IPNetwork.InternalSubnet(false, this, cidr, out ipnetworkCollection);
            return ipnetworkCollection;
        }


#if TRAVISCI
        public
#else
        internal
#endif


        static void InternalSubnet(bool trySubnet, IPNetwork network, byte cidr, out IPNetworkCollection ipnetworkCollection)
        {

            if (network == null)
            {
                if (trySubnet == false)
                {
                    throw new ArgumentNullException("network");
                }
                ipnetworkCollection = null;
                return;
            }

            int maxCidr = network._family == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;
            if (cidr > maxCidr)
            {
                if (trySubnet == false)
                {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                ipnetworkCollection = null;
                return;
            }

            if (cidr < network.Cidr)
            {
                if (trySubnet == false)
                {
                    throw new ArgumentException("cidr");
                }
                ipnetworkCollection = null;
                return;
            }

            ipnetworkCollection = new IPNetworkCollection(network, cidr);
            return;
        }

        public static Int32 Compare(IPNetwork left, IPNetwork right)
        {
            //  two null IPNetworks are equal
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return 0;

            //  two same IPNetworks are equal
            if (ReferenceEquals(left, right)) return 0;

            //  null is always sorted first
            if (ReferenceEquals(left, null)) return -1;
            if (ReferenceEquals(right, null)) return 1;

            //  first test family
            var result = left._family.CompareTo(right._family);
            if (result != 0) return result;

            //  second test the network
            result = left._network.CompareTo(right._network);
            if (result != 0) return result;

            //  then test the cidr
            result = left._cidr.CompareTo(right._cidr);
            return result;
        }

        public Int32 CompareTo(IPNetwork other)
        {
            return Compare(this, other);
        }

        public Int32 CompareTo(Object obj)
        {
            //  null is at less
            if (obj == null) return 1;

            //  convert to a proper Cidr object
            var other = obj as IPNetwork;

            //  type problem if null
            if (other == null)
            {
                throw new ArgumentException(
                    "The supplied parameter is an invalid type. Please supply an IPNetwork type.",
                    "obj");
            }

            //  perform the comparision
            return CompareTo(other);
        }


        public static Boolean operator ==(IPNetwork left, IPNetwork right)
        {
            return Equals(left, right);
        }

        public static Boolean operator !=(IPNetwork left, IPNetwork right)
        {
            return !Equals(left, right);
        }

        public static Boolean operator <(IPNetwork left, IPNetwork right)
        {
            return Compare(left, right) < 0;
        }

        public static Boolean operator >(IPNetwork left, IPNetwork right)
        {
            return Compare(left, right) > 0;
        }


        internal struct IPNetworkInteral
        {
            public BigInteger IPAddress;
            public byte Cidr;
            public AddressFamily AddressFamily;
        }


        IPNetwork(SerializationInfo info, StreamingContext context)
        {
            var sipnetwork = Convert.ToString(info.GetValue("IPNetwork", typeof(string)));
            var ipnetwork = IPNetwork.Parse(sipnetwork);

            this._ipaddress = ipnetwork._ipaddress;
            this._cidr = ipnetwork._cidr;
            this._family = ipnetwork._family;

        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IPNetwork", this.ToString());
        }

    }
}
