using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor>
{
    public TileBase[] tiles;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Grass = 3;
        public const int Path = 4;
        public const int Dirt = 5;
        public const int Sand = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
        case Neighbor.This:
            return tile == this || tile == null;
        case Neighbor.NotThis:
            return tile != this && tile != null;
        case Neighbor.Grass:
        case Neighbor.Path:
        case Neighbor.Dirt:
        case Neighbor.Sand:
            return tile == tiles[neighbor-Neighbor.Grass];
        }
        return base.RuleMatch(neighbor, tile);
    }
}