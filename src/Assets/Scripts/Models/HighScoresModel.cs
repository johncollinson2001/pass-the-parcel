using System.Xml.Serialization;

[XmlRoot("highScores")]
public class HighScoresModel
{
    [XmlElement("highScore")]
    public int HighScore;
}