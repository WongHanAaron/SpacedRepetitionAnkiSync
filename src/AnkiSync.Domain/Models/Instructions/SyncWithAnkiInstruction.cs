namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to synchronize with AnkiWeb.
/// </summary>
public class SyncWithAnkiInstruction : SynchronizationInstruction
{
    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.SyncWithAnki;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}";
}