namespace kinesisMyDataStreamFunction
{
    public class Rootobject
    {
        public Inputinformation InputInformation { get; set; }
        public Streamprocessorinformation StreamProcessorInformation { get; set; }
        public Facesearchresponse[] FaceSearchResponse { get; set; }
    }

    public class Inputinformation
    {
        public Kinesisvideo KinesisVideo { get; set; }
    }

    public class Kinesisvideo
    {
        public string StreamArn { get; set; }
        public string FragmentNumber { get; set; }
        public float ServerTimestamp { get; set; }
        public float ProducerTimestamp { get; set; }
        public float FrameOffsetInSeconds { get; set; }
    }

    public class Streamprocessorinformation
    {
        public string Status { get; set; }
    }

    public class Facesearchresponse
    {
        public Detectedface DetectedFace { get; set; }
        public Matchedface[] MatchedFaces { get; set; }
    }

    public class Detectedface
    {
        public Boundingbox BoundingBox { get; set; }
        public float Confidence { get; set; }
        public Landmark[] Landmarks { get; set; }
        public Pose Pose { get; set; }
        public Quality Quality { get; set; }
    }

    public class Boundingbox
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float Left { get; set; }
        public float Top { get; set; }
    }

    public class Pose
    {
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }
    }

    public class Quality
    {
        public float Brightness { get; set; }
        public float Sharpness { get; set; }
    }

    public class Landmark
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Type { get; set; }
    }

    public class Matchedface
    {
        public float Similarity { get; set; }
        public Face Face { get; set; }
    }

    public class Face
    {
        public Boundingbox1 BoundingBox { get; set; }
        public string FaceId { get; set; }
        public float Confidence { get; set; }
        public string ImageId { get; set; }
        public string ExternalImageId { get; set; }
    }

    public class Boundingbox1
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public float Left { get; set; }
        public float Top { get; set; }
    }
}
