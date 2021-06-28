using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor>
{
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        // public const int Any = 3;
        // public const int ThisOrNothing = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
        case Neighbor.This:
            return tile == this || tile == null;
        case Neighbor.NotThis:
            return tile != this;
        }
        return base.RuleMatch(neighbor, tile);
    }
}