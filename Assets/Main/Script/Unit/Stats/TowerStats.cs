namespace TD.Database
{
    public class TowerStats : UnitStats
    {
        public int level;
        public int cost;

        public TowerStats[] upgrade_path;
        public string tag;
    }
}