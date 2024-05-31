namespace TopUpManager.Common.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public ICollection<Beneficiary> Beneficiaries { get; set; }
    }
}
