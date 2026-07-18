namespace ProjectGenesis.Saving
{
    public interface IPlayerPersistence
    {
        bool TryLoad(out PlayerProfileData profile);
        bool Save(PlayerProfileData profile);
    }
}
