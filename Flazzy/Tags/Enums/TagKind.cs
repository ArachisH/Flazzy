namespace Flazzy.Tags
{
    public enum TagKind
    {
        End = 0,
        ShowFrame = 1,

        DefineShape = 2,
        DefineShape2 = 22,
        DefineShape3 = 32,
        DefineShape4 = 83,

        PlaceObject = 4,
        PlaceObject2 = 26,
        PlaceObject3 = 70,

        RemoveObject = 5,
        RemoveObject2 = 28,

        DefineBits = 6,

        DefineButton = 7,
        DefineButton2 = 34,

        JPEGTables = 8,
        SetBackgroundColor = 9,

        DefineFont = 10,
        DefineFont2 = 48,
        DefineFont3 = 75,
        DefineFont4 = 91,

        DefineText = 11,
        DefineText2 = 33,

        DoAction = 12,

        DefineFontInfo = 13,
        DefineFontInfo2 = 62,

        DefineSound = 14,

        StartSound = 15,
        StartSound2 = 89,

        DefineButtonSound = 17,

        SoundStreamHead = 18,
        SoundStreamHead2 = 45,

        SoundStreamBlock = 19,

        DefineBitsLossless = 20,
        DefineBitsLossless2 = 36,

        DefineBitsJPEG2 = 21,
        DefineBitsJPEG3 = 35,
        DefineBitsJPEG4 = 90,

        DefineButtonCxform = 23,
        Protect = 24,
        DefineEditText = 37,
        DefineSprite = 39,
        ProductInfo = 41,
        FrameLabel = 43,

        DefineMorphShape = 46,
        DefineMorphShape2 = 84,

        ExportAssets = 56,

        ImportAssets = 57,
        ImportAssets2 = 71,

        EnableDebugger = 58,
        EnableDebugger2 = 64,

        DoInitAction = 59,
        DefineVideoStream = 60,
        VideoFrame = 61,
        ScriptLimits = 65,
        SetTabIndex = 66,
        FileAttributes = 69,
        DefineFontAlignZones = 73,
        CSMTextSettings = 74,
        SymbolClass = 76,
        Metadata = 77,
        DefineScalingGrid = 78,
        DoABC = 82,
        DefineSceneAndFrameLabelData = 86,
        DefineBinaryData = 87,
        DefineFontName = 88,
        EnableTelemetry = 93,

        Unknown = 255
    }
}