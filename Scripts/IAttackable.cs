using System;

interface IAttackable {
    // handle being hit; optionally with vampiric strength
    void Attacked(bool vampiricStrength);
}