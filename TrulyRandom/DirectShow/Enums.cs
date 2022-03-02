using System;

// Source: AForge library
namespace TrulyRandom.DirectShow
{
    enum PinDirection
    {
        /// <summary>
        /// Input pin.
        /// </summary>
        Input,

        /// <summary>
        /// Output pin.
        /// </summary>
        Output
    }

    [Flags]
    enum VideoControlFlags
    {
        FlipHorizontal = 0x0001,
        FlipVertical = 0x0002,
        ExternalTriggerEnable = 0x0004,
        Trigger = 0x0008
    }

    enum DsEvCode
    {
        None,
        Complete = 0x01,      // EC_COMPLETE
        DeviceLost = 0x1F,      // EC_DEVICE_LOST
        //(...) not yet interested in other events
    }

    enum PhysicalConnectorType
    {
        /// <summary>
        /// Default value of connection type. Physically it does not exist, but just either to specify that
        /// connection type should not be changed (input) or was not determined (output).
        /// </summary>
        Default = 0,
        /// <summary>
        /// Specifies a tuner pin for video.
        /// </summary>
        VideoTuner = 1,
        /// <summary>
        /// Specifies a composite pin for video.
        /// </summary>
        VideoComposite,
        /// <summary>
        /// Specifies an S-Video (Y/C video) pin.
        /// </summary>
        VideoSVideo,
        /// <summary>
        /// Specifies an RGB pin for video.
        /// </summary>
        VideoRGB,
        /// <summary>
        /// Specifies a YRYBY (Y, R–Y, B–Y) pin for video.
        /// </summary>
        VideoYRYBY,
        /// <summary>
        /// Specifies a serial digital pin for video.
        /// </summary>
        VideoSerialDigital,
        /// <summary>
        /// Specifies a parallel digital pin for video.
        /// </summary>
        VideoParallelDigital,
        /// <summary>
        /// Specifies a SCSI (Small Computer System Interface) pin for video.
        /// </summary>
        VideoSCSI,
        /// <summary>
        /// Specifies an AUX (auxiliary) pin for video.
        /// </summary>
        VideoAUX,
        /// <summary>
        /// Specifies an IEEE 1394 pin for video.
        /// </summary>
        Video1394,
        /// <summary>
        /// Specifies a USB (Universal Serial Bus) pin for video.
        /// </summary>
        VideoUSB,
        /// <summary>
        /// Specifies a video decoder pin.
        /// </summary>
        VideoDecoder,
        /// <summary>
        /// Specifies a video encoder pin.
        /// </summary>
        VideoEncoder,
        /// <summary>
        /// Specifies a SCART (Peritel) pin for video.
        /// </summary>
        VideoSCART,
        /// <summary>
        /// Not used.
        /// </summary>
        VideoBlack,

        /// <summary>
        /// Specifies a tuner pin for audio.
        /// </summary>
        AudioTuner = 4096,
        /// <summary>
        /// Specifies a line pin for audio.
        /// </summary>
        AudioLine,
        /// <summary>
        /// Specifies a microphone pin.
        /// </summary>
        AudioMic,
        /// <summary>
        /// Specifies an AES/EBU (Audio Engineering Society/European Broadcast Union) digital pin for audio.
        /// </summary>
        AudioAESDigital,
        /// <summary>
        /// Specifies an S/PDIF (Sony/Philips Digital Interface Format) digital pin for audio.
        /// </summary>
        AudioSPDIFDigital,
        /// <summary>
        /// Specifies a SCSI pin for audio.
        /// </summary>
        AudioSCSI,
        /// <summary>
        /// Specifies an AUX pin for audio.
        /// </summary>
        AudioAUX,
        /// <summary>
        /// Specifies an IEEE 1394 pin for audio.
        /// </summary>
        Audio1394,
        /// <summary>
        /// Specifies a USB pin for audio.
        /// </summary>
        AudioUSB,
        /// <summary>
        /// Specifies an audio decoder pin.
        /// </summary>
        AudioDecoder
    }

    static class PinCategory
    {
        /// <summary>
        /// Capture pin.
        /// </summary>
        /// 
        /// <remarks>Equals to PIN_CATEGORY_CAPTURE.</remarks>
        /// 
        public static readonly Guid Capture =
            new Guid(0xFB6C4281, 0x0353, 0x11D1, 0x90, 0x5F, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA);

        /// <summary>
        /// Still image pin.
        /// </summary>
        /// 
        /// <remarks>Equals to PIN_CATEGORY_STILL.</remarks>
        /// 
        public static readonly Guid StillImage =
            new Guid(0xFB6C428A, 0x0353, 0x11D1, 0x90, 0x5F, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA);
    }

    static class MediaType
    {
        /// <summary>
        /// Video.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Video.</remarks>
        /// 
        public static readonly Guid Video =
            new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// Interleaved. Used by Digital Video (DV).
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Interleaved.</remarks>
        /// 
        public static readonly Guid Interleaved =
            new Guid(0x73766169, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// Audio.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Audio.</remarks>
        /// 
        public static readonly Guid Audio =
            new Guid(0x73647561, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// Text.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Text.</remarks>
        /// 
        public static readonly Guid Text =
            new Guid(0x73747874, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// Byte stream with no time stamps.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIATYPE_Stream.</remarks>
        /// 
        public static readonly Guid Stream =
            new Guid(0xE436EB83, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
    }

    static class MediaSubType
    {
        /// <summary>
        /// YUY2 (packed 4:2:2).
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_YUYV.</remarks>
        /// 
        public static readonly Guid YUYV =
            new Guid(0x56595559, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// IYUV.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_IYUV.</remarks>
        /// 
        public static readonly Guid IYUV =
            new Guid(0x56555949, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// A DV encoding format. (FOURCC 'DVSD')
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_DVSD.</remarks>
        /// 
        public static readonly Guid DVSD =
            new Guid(0x44535644, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

        /// <summary>
        /// RGB, 1 bit per pixel (bpp), palettized.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB1.</remarks>
        /// 
        public static readonly Guid RGB1 =
            new Guid(0xE436EB78, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// RGB, 4 bpp, palettized.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB4.</remarks>
        /// 
        public static readonly Guid RGB4 =
            new Guid(0xE436EB79, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// RGB, 8 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB8.</remarks>
        /// 
        public static readonly Guid RGB8 =
            new Guid(0xE436EB7A, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// RGB 565, 16 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB565.</remarks>
        /// 
        public static readonly Guid RGB565 =
            new Guid(0xE436EB7B, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// RGB 555, 16 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB555.</remarks>
        /// 
        public static readonly Guid RGB555 =
            new Guid(0xE436EB7C, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// RGB, 24 bpp.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB24.</remarks>
        /// 
        public static readonly Guid RGB24 =
            new Guid(0xE436Eb7D, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// RGB, 32 bpp, no alpha channel.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_RGB32.</remarks>
        /// 
        public static readonly Guid RGB32 =
            new Guid(0xE436EB7E, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// Data from AVI file.
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_Avi.</remarks>
        /// 
        public static readonly Guid Avi =
            new Guid(0xE436EB88, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// Advanced Streaming Format (ASF).
        /// </summary>
        /// 
        /// <remarks>Equals to MEDIASUBTYPE_Asf.</remarks>
        /// 
        public static readonly Guid Asf =
            new Guid(0x3DB80F90, 0x9412, 0x11D1, 0xAD, 0xED, 0x00, 0x00, 0xF8, 0x75, 0x4B, 0x99);
    }

    static class FindDirection
    {
        /// <summary>Equals to LOOK_UPSTREAM_ONLY.</summary>
        public static readonly Guid UpstreamOnly =
            new Guid(0xAC798BE0, 0x98E3, 0x11D1, 0xB3, 0xF1, 0x00, 0xAA, 0x00, 0x37, 0x61, 0xC5);

        /// <summary>Equals to LOOK_DOWNSTREAM_ONLY.</summary>
        public static readonly Guid DownstreamOnly =
            new Guid(0xAC798BE1, 0x98E3, 0x11D1, 0xB3, 0xF1, 0x00, 0xAA, 0x00, 0x37, 0x61, 0xC5);
    }

    [Flags]
    enum AnalogVideoStandard
    {
        None = 0x00000000,   // This is a digital sensor
        NTSC_M = 0x00000001,   //        75 IRE Setup
        NTSC_M_J = 0x00000002,   // Japan,  0 IRE Setup
        NTSC_433 = 0x00000004,
        PAL_B = 0x00000010,
        PAL_D = 0x00000020,
        PAL_G = 0x00000040,
        PAL_H = 0x00000080,
        PAL_I = 0x00000100,
        PAL_M = 0x00000200,
        PAL_N = 0x00000400,
        PAL_60 = 0x00000800,
        SECAM_B = 0x00001000,
        SECAM_D = 0x00002000,
        SECAM_G = 0x00004000,
        SECAM_H = 0x00008000,
        SECAM_K = 0x00010000,
        SECAM_K1 = 0x00020000,
        SECAM_L = 0x00040000,
        SECAM_L1 = 0x00080000,
        PAL_N_COMBO = 0x00100000    // Argentina
    }

    static class FormatType
    {
        /// <summary>
        /// VideoInfo.
        /// </summary>
        /// 
        /// <remarks>Equals to FORMAT_VideoInfo.</remarks>
        /// 
        public static readonly Guid VideoInfo =
            new Guid(0x05589F80, 0xC356, 0x11CE, 0xBF, 0x01, 0x00, 0xAA, 0x00, 0x55, 0x59, 0x5A);

        /// <summary>
        /// VideoInfo2.
        /// </summary>
        /// 
        /// <remarks>Equals to FORMAT_VideoInfo2.</remarks>
        /// 
        public static readonly Guid VideoInfo2 =
            new Guid(0xf72A76A0, 0xEB0A, 0x11D0, 0xAC, 0xE4, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA);

        public static readonly Guid WaveFormatEx =
            new Guid(0x05589f81, 0xc356, 0x11CE, 0xBF, 0x01, 0x00, 0xAA, 0x00, 0x55, 0x59, 0x5A);
    }
}
