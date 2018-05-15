namespace MHXXSaveEditor.Data
{
    public class Player
    {
        public int SaveOffset { get; set; }

        //General Info
        public string Name { get; set; }
        public int PlayTime { get; set; }
        public int Funds { get; set; }
        public int HunterRank { get; set; }

        //Character Details
        public byte Voice { get; set; }
        public byte EyeColor { get; set; }
        public byte Clothing { get; set; }
        public byte Gender { get; set; }
        //public byte HuntingStyle { get; set; }
        public byte HairStyle { get; set; }
        public byte Face { get; set; }
        public byte Features { get; set; }

        //Character Colors
        public byte[] SkinColorRGBA { get; set; }
        public byte[] HairColorRGBA { get; set; }
        public byte[] FeaturesColorRGBA { get; set; }
        public byte[] ClothingColorRGBA { get; set; }

        //Points
        public int HRPoints { get; set; }
        public int AcademyPoints { get; set; }
        public int BhernaPoints { get; set; }
        public int KokotoPoints { get; set; }
        public int PokkePoints { get; set; }
        public int YukumoPoints { get; set; }

        // Monster Hunts / Sizes
        public byte[] MonsterKills { get; set; }
        public byte[] MonsterCaptures { get; set; }
        public byte[] MonsterSizes { get; set; }

        //Item Box
        public string[] ItemId { get; set; }
        public string[] ItemCount { get; set; }

        //Equipment Box
        public byte[] EquipmentInfo { get; set; }

        // Palico Equipment Box
        public byte[] EquipmentPalico { get; set; }

        // Palico
        public byte[] PalicoData { get; set; }

        // Guild Card
        public byte[] GuildCardData { get; set; }
        public byte[] ArenaData { get; set; }

        // Shoutouts
        public byte[] AutomaticShoutouts { get; set; }
        public byte[] ManualShoutouts { get; set; }
    }
}
