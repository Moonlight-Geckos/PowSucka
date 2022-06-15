public interface IDisposable
{
    public void Dispose();
}
public interface IDamagable
{
    public void GetDamage(float damage);
}
public interface ISuckable
{
    public void GetSucked();
}