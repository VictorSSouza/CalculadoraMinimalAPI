using Microsoft.EntityFrameworkCore;
using CalculatorAPI.Models;

namespace CalculatorAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CalculationHistory> CalculationHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações adicionais do modelo podem ser feitas aqui, se necessário
        modelBuilder.Entity<CalculationHistory>(entity =>
        {
            entity.HasKey(e => e.Id); // Define a chave primária
            entity.Property(e => e.LeftOperand).IsRequired(); // Define que o operando esquerdo é obrigatório
            entity.Property(e => e.RightOperand).IsRequired(); // Define que o operando direito
            entity.Property(e => e.Operator).IsRequired(); // Define que o operador é obrigatório
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired(); // Define valor padrão para CreatedAt
        });
    }
}
