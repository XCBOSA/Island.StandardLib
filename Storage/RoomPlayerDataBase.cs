using Island.StandardLib.Math;

namespace Island.StandardLib.Storage
{
    public class RoomPlayerDataBase : IStorable
    {
        public long PeopleCount;
        public float PeopleHappy;
        public Percentage EcoStructure;
        public float Inflation;
        public long Foods;
        public long Technology;
        public long MedicalTreatment;
        public long Army;
        public long Education;
        public long Achievements;
        public long GovIncome;

        public int GDPperPeople => (int)((EcoStructure[0] * 5f + EcoStructure[1] * 30 + EcoStructure[2] * 100) * PeopleHappy);
        public long GDP => GDPperPeople * PeopleCount;

        public void ReadFromData(DataStorage data)
        {
            data.Read(out PeopleCount);
            data.Read(out PeopleHappy);
            data.Read(out EcoStructure);
            data.Read(out Inflation);
            data.Read(out Foods);
            data.Read(out Technology);
            data.Read(out MedicalTreatment);
            data.Read(out Army);
            data.Read(out Education);
            data.Read(out Achievements);
            data.Read(out GovIncome);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(PeopleCount);
            data.Write(PeopleHappy);
            data.Write(EcoStructure);
            data.Write(Inflation);
            data.Write(Foods);
            data.Write(Technology);
            data.Write(MedicalTreatment);
            data.Write(Army);
            data.Write(Education);
            data.Write(Achievements);
            data.Write(GovIncome);
        }
    }
}
