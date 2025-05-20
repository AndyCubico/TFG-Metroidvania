public interface IDamagable 
{
    void Damage(float damage);
    void Die();

    float m_MaxHealth {  get; set; }
    float currentHealth { get; set; }
}
