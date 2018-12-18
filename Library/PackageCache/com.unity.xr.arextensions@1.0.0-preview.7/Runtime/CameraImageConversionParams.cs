using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.ARExtensions
{
    /// <summary>
    /// Describes a set of conversion parameters for use with <see cref="CameraImage"/>'s conversion methods.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraImageConversionParams : IEquatable<CameraImageConversionParams>
    {
        RectInt m_InputRect;
        Vector2Int m_OutputDimensions;
        TextureFormat m_Format;
        CameraImageTransformation m_Transformation;

        /// <summary>
        /// The portion of the original image that will be converted to <see cref="format"/>.
        /// The input rectangle must be completely contained inside the <see cref="CameraImage"/>'s dimensions.
        /// </summary>
        public RectInt inputRect
        {
            get { return m_InputRect; }
            set { m_InputRect = value; }
        }

        /// <summary>
        /// The dimensions of the converted image. The dimensions must be less than or equal to the <see cref="inputRect"/>'s width and height.
        /// If the dimensions are less than the <see cref="inputRect"/>'s width and height, downsampling is performed using nearest neighbor.
        /// </summary>
        public Vector2Int outputDimensions
        {
            get { return m_OutputDimensions; }
            set { m_OutputDimensions = value; }
        }

        /// <summary>
        /// The <c>TextureFormat</c> to convert to. See <see cref="CameraImage.FormatSupported(TextureFormat)"/>
        /// for a list of supported formats.
        /// </summary>
        public TextureFormat outputFormat
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        /// <summary>
        /// The transformation to apply to the image during conversion.
        /// </summary>
        public CameraImageTransformation transformation
        {
            get { return m_Transformation; }
            set { m_Transformation =  value; }
        }

        /// <summary>
        /// Constructs a <see cref="CameraImageConversionParams"/> using the <paramref name="image"/>'s full resolution. That is,
        /// it sets <see cref="inputRect"/> to <c>(0, 0, image.width, image.height)</c> and <see cref="outputDimensions"/> to <c>(image.width, image.height)</c>.
        /// </summary>
        /// <param name="image">The source <see cref="CameraImage"/>.</param>
        /// <param name="format">The <c>TextureFormat</c> to convert to.</param>
        /// <param name="transformation">An <see cref="CameraImageTransformation"/> to apply (optional).</param>
        public CameraImageConversionParams(
            CameraImage image,
            TextureFormat format,
            CameraImageTransformation transformation = CameraImageTransformation.None)
        {
            m_InputRect = new RectInt(0, 0, image.width, image.height);
            m_OutputDimensions = new Vector2Int(image.width, image.height);
            m_Format = format;
            m_Transformation = transformation;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = inputRect.GetHashCode();
                hash = hash * 486187739 + outputDimensions.GetHashCode();
                hash = hash * 486187739 + outputFormat.GetHashCode();
                hash = hash * 486187739 + transformation.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CameraImageConversionParams))
                return false;

            return Equals((CameraImageConversionParams)obj);
        }

        public override string ToString()
        {
            return string.Format(
                "inputRect: {0} outputDimensions: {1} format: {2} transformation: {3})",
                inputRect,
                outputDimensions,
                outputFormat,
                transformation);
        }

        public bool Equals(CameraImageConversionParams other)
        {
            return
                (inputRect.Equals(other.inputRect)) &&
                (outputDimensions.Equals(other.outputDimensions)) &&
                (outputFormat == other.outputFormat) &&
                (transformation == other.transformation);
        }

        public static bool operator ==(CameraImageConversionParams lhs, CameraImageConversionParams rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CameraImageConversionParams lhs, CameraImageConversionParams rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
