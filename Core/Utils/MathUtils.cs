using System;
using System.Runtime.CompilerServices;
using T3.Core.Animation;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace T3.Core.Utils
{
    public static class MathUtils
    {
        public static float ToRad => (float)(Math.PI / 180.0);
        public static float ToDegree => (float)(180.0 / Math.PI);

        public static float PerlinNoise(float value, float period, int octaves, int seed)
        {
            var noiseSum = 0.0f;

            var frequency = period;
            var amplitude = 0.5f;
            for (var octave = 0; octave < octaves - 1; octave++)
            {
                var v = value * frequency + seed * 12.468f;
                var a = Noise((int)v, seed);
                var b = Noise((int)v + 1, seed);
                var t = Fade(v - (float)Math.Floor(v));
                noiseSum += SharpDX.MathUtil.Lerp(a, b, t) * amplitude;
                frequency *= 2;
                amplitude *= 0.5f;
            }

            return noiseSum;
        }

        private static float Noise(int x, int seed)
        {
            int n = x + seed * 137;
            n = (n << 13) ^ n;
            return (float)(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
        }

        public static uint XxHash(uint p)
        {
            const uint PRIME32_2 = 2246822519U, PRIME32_3 = 3266489917U;
            const uint PRIME32_4 = 668265263U, PRIME32_5 = 374761393U;

            uint h32 = p + PRIME32_5;
            h32 = PRIME32_4 * ((h32 << 17) | (h32 >> (32 - 17)));
            h32 = PRIME32_2 * (h32 ^ (h32 >> 15));
            h32 = PRIME32_3 * (h32 ^ (h32 >> 13));

            return h32 ^ (h32 >> 16);
        }

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static float SmootherStep(float min, float max, float value)
        {
            var t = Math.Max(0, Math.Min(1, (value - min) / (max - min)));
            return Fade(t);
        }

        private static float SmoothStep(float min, float max, float value)
        {
            var x = Math.Max(0, Math.Min(1, (value - min) / (max - min)));
            return x * x * (3 - 2 * x);
        }

        private static double SmoothStep(double min, double max, double value)
        {
            var x = Math.Max(0, Math.Min(1, (value - min) / (max - min)));
            return x * x * (3 - 2 * x);
        }

        public static Vector2 Clamp(Vector2 v, Vector2 mn, Vector2 mx)
        {
            return new Vector2((v.X < mn.X)
                                   ? mn.X
                                   : (v.X > mx.X)
                                       ? mx.X
                                       : v.X, (v.Y < mn.Y) ? mn.Y : (v.Y > mx.Y) ? mx.Y : v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(T lhs, T rhs) where T : IComparable<T>
        {
            return lhs.CompareTo(rhs) < 0 ? lhs : rhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(T lhs, T rhs) where T : IComparable<T>
        {
            return lhs.CompareTo(rhs) >= 0 ? lhs : rhs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static int ClampForEnum<T>(this int i) where T:Enum 
        {
            return i.Clamp(0, Enum.GetValues(typeof(T)).Length - 1);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            return (float)(a + (b - a) * t);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Fmod(float v, float mod)
        {
            return v - mod * (float)Math.Floor(v / mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Fmod(double v, double mod)
        {
            return v - mod * Math.Floor(v / mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeAndClamp(float value, float min, float max)
        {
            return MathF.Max(0, MathF.Min(1,(value - min) / (max - min)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NormalizeAndClamp(double value, double min, double max)
        {
            return Math.Max(0, Math.Min(1,(value - min) / (max - min)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RemapAndClamp(float value, float inMin, float inMax, float outMin, float outMax)
        {
            var factor = (value - inMin) / (inMax - inMin);
            var v = factor * (outMax - outMin) + outMin;
            if (outMin > outMax)
                Utilities.Swap(ref outMin, ref outMax);
            return v.Clamp(outMin, outMax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            var factor = (value - inMin) / (inMax - inMin);
            var v = factor * (outMax - outMin) + outMin;
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RemapAndClamp(double value, double inMin, double inMax, double outMin, double outMax)
        {
            var factor = (value - inMin) / (inMax - inMin);
            var v = factor * (outMax - outMin) + outMin;
            if (v > outMax)
            {
                v = outMax;
            }
            else if (v < outMin)
            {
                v = outMin;
            }

            return v;
        }

        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X < rhs.X ? lhs.X : rhs.X, lhs.Y < rhs.Y ? lhs.Y : rhs.Y);
        }

        public static Vector2 Floor(Vector2 v)
        {
            return new Vector2((float)Math.Floor(v.X), (float)Math.Floor(v.Y));
        }

        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X >= rhs.X ? lhs.X : rhs.X, lhs.Y >= rhs.Y ? lhs.Y : rhs.Y);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(a.X + (b.X - a.X) * t,
                               a.Y + (b.Y - a.Y) * t,
                               a.Z + (b.Z - a.Z) * t);
        }

        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(a.X + (b.X - a.X) * t,
                               a.Y + (b.Y - a.Y) * t,
                               a.Z + (b.Z - a.Z) * t,
                               a.W + (b.W - a.W) * t);
        }

        public static double Lerp(double a, double b, double t)
        {
            return (double)(a + (b - a) * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lerp(int a, int b, float t)
        {
            return (int)(a + (b - a) * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Log2(double value)
        {
            return Math.Log10(value) / Math.Log10(2.0);
        }

        public static float RoundValue(float i, float stepsPerUnit, float stepRatio)
        {
            float u = 1 / stepsPerUnit;
            float v = stepRatio / (2 * stepsPerUnit);
            float m = i % u;
            float r = m - (m < v
                               ? 0
                               : (m > (u - v))
                                   ? u
                                   : ((m - v) / (1 - 2 * stepsPerUnit * v)));
            float y = i - r;
            return y;
        }

        /// <summary>
        /// Smooth damps a value with a "critically damped spring" similar to unity's SmoothDamp helper method.
        /// See https://stackoverflow.com/a/5100956 
        /// </summary>
        public static float SpringDamp(float target,
                                       float current,
                                       ref float velocity,
                                       float springConstant = 2,
                                       float timeStep = 1 / 60f)
        {
            //const float springConstant = 0.41f;
            var currentToTarget = target - current;
            var springForce = currentToTarget * springConstant;
            var dampingForce = -velocity * 2 * MathF.Sqrt(springConstant);
            var force = springForce + dampingForce;
            velocity += force * timeStep;
            var displacement = velocity * timeStep;
            return current + displacement;
        }

        public const float Pi2 = (float)Math.PI * 2;

        public static Vector3 ToVector3(this Vector4 vec)
        {
            return new Vector3(vec.X / vec.W, vec.Y / vec.W, vec.Z / vec.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharpDX.Vector3 ToVector3(this SharpDX.Vector4 vec)
        {
            return new SharpDX.Vector3(vec.X / vec.W, vec.Y / vec.W, vec.Z / vec.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharpDX.Vector3 ToSharpDx(this Vector3 source)
        {
            return new SharpDX.Vector3(source.X, source.Y, source.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToNumerics(this SharpDX.Vector3 source)
        {
            return new Vector3(source.X, source.Y, source.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharpDX.Vector4 ToSharpDx(this Vector4 source)
        {
            return new SharpDX.Vector4(source.X, source.Y, source.Z, source.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToNumerics(this SharpDX.Vector4 source)
        {
            return new Vector4(source.X, source.Y, source.Z, source.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharpDX.Vector4 ToSharpDxVector4(this Vector3 source, float w)
        {
            return new SharpDX.Vector4(source.X, source.Y, source.Z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharpDX.Vector3 ToSharpDxVector3(this Vector3 source)
        {
            return new SharpDX.Vector3(source.X, source.Y, source.Z);
        }

        /// <summary>
        /// Return true if a boolean changed from false to true
        /// </summary>
        public static bool WasTriggered(bool newState, ref bool current)
        {
            if (newState == current)
                return false;

            current = newState;
            return newState;
        }

        /// <summary>
        /// Return true if a boolean changed from false to true
        /// </summary>
        public static bool WasReleased(bool newState, ref bool current)
        {
            if (newState == current)
                return false;

            current = newState;
            return !newState;
        }

        /// <summary>
        /// Checks for NaN or Infinity, and sets the float to the provided default value if either.
        /// </summary>
        /// <returns>True if NaN or Infinity</returns>
        public static bool ApplyDefaultIfInvalid(ref float val, float defaultValue)
        {
            var isInvalid = float.IsNaN(val) || float.IsInfinity(val);
            val = isInvalid ? defaultValue : val;
            return isInvalid;
        }

        public static bool ApplyDefaultIfInvalid(ref Vector2 val, Vector2 defaultValue)
        {
            var isInvalid = float.IsNaN(val.X) || float.IsInfinity(val.X) ||
                            float.IsNaN(val.Y) || float.IsInfinity(val.Y);
            val = isInvalid ? defaultValue : val;
            return isInvalid;
        }
        
        public static bool ApplyDefaultIfInvalid(ref Vector3 val, Vector3 defaultValue)
        {
            var isInvalid = float.IsNaN(val.X) || float.IsInfinity(val.X) ||
                            float.IsNaN(val.Y) || float.IsInfinity(val.Y) ||
                            float.IsNaN(val.Z) || float.IsInfinity(val.Z);
            val = isInvalid ? defaultValue : val;
            return isInvalid;
        }

        /// <summary>
        /// Checks for NaN or Infinity, and sets the double to the provided default value if either.
        /// </summary>
        /// <returns>True if NaN or Infinity</returns>
        public static bool ApplyDefaultIfInvalid(ref double val, double defaultValue)
        {
            bool isInvalid = double.IsNaN(val) || double.IsInfinity(val);
            val = isInvalid ? defaultValue : val;
            return isInvalid;
        }

        public static Quaternion RotationFromTwoPositions(Vector3 p1, Vector3 p2)
        {
            return Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), (float)(Math.Atan2(p1.X - p2.X, -(p1.Y - p2.Y)) + Math.PI / 2));
        }
    }

    public class EaseFunctions
    {
        public static float EaseOutElastic(float x)
        {
            const float c4 = (float)(2 * Math.PI) / 3;

            return x <= 0f
                       ? 0f
                       : x >= 1f
                           ? 1f
                           : (float)(Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
        }
    }

    public static class DampFunctions
    {
        public enum Methods
        {
            LinearInterpolation,
            DampedSpring
        }

        public static float DampenFloat(float inputValue, float previousValue, float damping, ref float velocity, Methods method)
        {
            return method switch
                       {
                           Methods.LinearInterpolation => LinearDamp(inputValue, previousValue, damping),
                           Methods.DampedSpring        => SpringDampFloat(inputValue, previousValue, damping, ref velocity),
                           _                           => inputValue
                       };
        }

        public static float SpringDampFloat(float inputValue, float previousValue, float damping, ref float velocity)
        {
            return MathUtils.SpringDamp(inputValue, previousValue, ref velocity, 0.5f / (damping + 0.001f), (float)Playback.LastFrameDuration);
        }

        private static float LinearDamp(float targetValue, float currentValue, float damping)
        {
            // TODO: Fix damping factor from framerate 
            return MathUtils.Lerp(targetValue, currentValue, damping);
        }
        
        public static Vector2 SpringDampVec2(Vector2 targetVec, Vector2 currentValue, float damping, ref Vector2 velocity)
        {
            var dt = (float)Playback.LastFrameDuration;
            return new Vector2(
                               MathUtils.SpringDamp(targetVec.X, currentValue.X, ref velocity.X, 0.5f / (damping + 0.001f), dt),
                               MathUtils.SpringDamp(targetVec.Y, currentValue.Y, ref velocity.Y, 0.5f / (damping + 0.001f), dt));
        }

        public static Vector3 SpringDampVec3(Vector3 targetVec, Vector3 currentValue, float damping, ref Vector3 velocity)
        {
            var dt = (float)Playback.LastFrameDuration;
            return new Vector3(
                               MathUtils.SpringDamp(targetVec.X, currentValue.X, ref velocity.X, 0.5f / (damping + 0.001f), dt),
                               MathUtils.SpringDamp(targetVec.Y, currentValue.Y, ref velocity.Y, 0.5f / (damping + 0.001f), dt),
                               MathUtils.SpringDamp(targetVec.Z, currentValue.Z, ref velocity.Z, 0.5f / (damping + 0.001f), dt));
        }
    }
}