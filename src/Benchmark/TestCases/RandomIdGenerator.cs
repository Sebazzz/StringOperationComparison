namespace Benchmark.TestCases {
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using BenchmarkDotNet.Attributes;

    public class RandomIdGenerator {
        private readonly Guid _id;

        private static readonly char[] Chars = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public RandomIdGenerator() {
            this._id = Guid.NewGuid();
        }

        [Benchmark]
        public string UsingStringConcatenation() {
            // Convert guid to bytes
            byte[] bytes = this._id.ToByteArray();

            // Target string
            int sizeOfGuid = bytes.Length;
            string result = String.Empty;

            for (int i = 0; i < sizeOfGuid; i++) {
                byte src = bytes[i];
                int carry;

                {
                    int index = src % Chars.Length;

                    char current = Chars[index];

                    result += current;

                    carry = ((src - index) / Chars.Length) - 1;
                }

                if (carry > 0) {
                    int index = carry % Chars.Length;

                    char current = Chars[index];

                    result += current;
                }
            }

            return result;
        }

        [Benchmark]
        public string UsingStringBuilderWithPredefinedCapacity() => this.UsingStringBuilder(true);
        [Benchmark]
        public string UsingStringBuilderWithoutPredefinedCapacity() => this.UsingStringBuilder(false);

        private string UsingStringBuilder(bool reserveCapacity) {
            // Convert guid to bytes
            byte[] bytes = this._id.ToByteArray();

            // Target string
            int sizeOfGuid = bytes.Length;
            int size = sizeOfGuid * 2;
            StringBuilder stringBuilder = reserveCapacity ? new StringBuilder(size) : new StringBuilder();

            for (int i = 0; i < sizeOfGuid; i++) {
                byte src = bytes[i];
                int carry;

                {
                    int index = src % Chars.Length;

                    char current = Chars[index];

                    stringBuilder.Append(current);

                    carry = ((src - index) / Chars.Length) - 1;
                }

                if (carry > 0) {
                    int index = carry % Chars.Length;

                    char current = Chars[index];

                    stringBuilder.Append(current);
                }
            }

            return stringBuilder.ToString();
        }

        [Benchmark]
        public unsafe string UsingUnsafePointers() {
            // Convert Guid to bytes
            GuidBuffer buffer = new GuidBuffer(this._id);
            byte* bytes = buffer.buffer;

            // Target string
            int sizeOfGuid = sizeof(Guid);
            int size =  sizeOfGuid * 2;
            char* result = stackalloc char[size + 1 /* \0 terminator */];
            char* start = result;

            for (int i = 0; i < sizeOfGuid; i++) {
                byte src = *bytes;
                int carry = 0;

                {
                    int index = src % Chars.Length;

                    char current = Chars[index];

                    *result = current;
                    result++;

                    carry = ((src - index) / Chars.Length) - 1;
                }

                if (carry > 0) {
                    int index = carry % Chars.Length;

                    char current = Chars[index];

                    *result = current;
                    result++;
                }

                bytes++;
            }

            return new string(start);
        }

        [StructLayout(LayoutKind.Explicit)]
        unsafe struct GuidBuffer {
            [FieldOffset(0)]
            public fixed byte buffer[16];

            [FieldOffset(0)]
            public Guid Guid;

            public GuidBuffer(Guid guid) : this() {
                this.Guid = guid;
            }
        }
    }
}