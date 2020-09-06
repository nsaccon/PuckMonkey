using System;
using System.Collections.Generic;
using System.Text;

namespace PuckMonkey
{
    public class PointSet
    {
        public int TimesAchieved;
        private int lastX;
        private PointSetType type;
        public PointSet(PointSetType type, int lastX)
        {
            this.type = type;
            this.lastX = lastX;
        }
        public double DraftKingsAvg() => Math.Round(DraftKings() / lastX, 4, MidpointRounding.AwayFromZero);
        private double DraftKings() =>
            type switch
            {
                PointSetType.GOAL => TimesAchieved * 8.5,
                PointSetType.ASSIST => TimesAchieved * 5,
                PointSetType.SHOT => TimesAchieved * 1.5,
                PointSetType.BLOCK => TimesAchieved * 1.3,
                PointSetType.SHORTHANDEDPOINT => TimesAchieved * 2,
                PointSetType.SHOOTOUTGOAL => TimesAchieved * 1.5,
                PointSetType.HATTRICK => TimesAchieved * 3,
                PointSetType.SHOTS5PLUS => TimesAchieved * 3,
                PointSetType.BLOCKS3PLUS => TimesAchieved * 3,
                PointSetType.POINTS3PLUS => TimesAchieved * 3,
                PointSetType.WIN => TimesAchieved * 6,
                PointSetType.SAVE => TimesAchieved * 0.7,
                PointSetType.GOALAGAINST => TimesAchieved * -3.5,
                PointSetType.SHUTOUT => TimesAchieved * 4,
                PointSetType.OVERTIMELOSS => TimesAchieved * 2,
                PointSetType.SAVES35PLUS => TimesAchieved * 3,
                _ => 0
            };
    }

    public enum PointSetType
    {
        GOAL,
        ASSIST,
        SHOT,
        BLOCK,
        SHORTHANDEDPOINT,
        SHOOTOUTGOAL,
        HATTRICK,
        SHOTS5PLUS,
        BLOCKS3PLUS,
        POINTS3PLUS,
        WIN,
        SAVE,
        GOALAGAINST,
        SHUTOUT,
        OVERTIMELOSS,
        SAVES35PLUS
    }
}
