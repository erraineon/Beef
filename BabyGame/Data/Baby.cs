namespace BabyGame.Data;

public abstract class Baby
{
    public Guid Id { get; init; }
    public required Marriage Marriage { get; set; }
    public required string Name { get; set; }
    public required double TotalExperience { get; set; }

    /// <summary>
    ///     Between 1 and 15
    ///     F, E, D, C, B, A, S, SS, SSS, ⭐, ⭐⭐, ⭐⭐⭐, etc.
    ///     Follows formula: f\left(x\right)=x^{3}
    /// </summary>
    public required int Level { get; set; } = 1;
    public required DateTimeOffset BirthDate { get; set; }
    public DateTimeOffset? GraduationDate { get; set; }
}