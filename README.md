![](icon.png)
# SanguinePact for V Rising

SanguineArchives is a server mod for V Rising. Trade defense for offense.

You can reach out to me on the V Rising Mod Community Discord (dokebi) for issues, questions, or suggestions.

## Commands
| COMMAND | SHORTHAND             | DESCRIPTION |
|---------|-----------------------|-------------|
| `.sanguinepact toggle` | `.sgp t`              | Toggle Sanguine Pact on or off. |
| `.sanguinepact toggleplayer (PlayerName)` | `.sgp p (PlayerName)` | Toggle Sanguine Pact on or off for player. |
| `.sanguinepact list` | `.sgp l`              | List players in Sanguine Pact. |

## Configuration

Once the mod installed, a configuration file will be created in the \BepInEx\config server folder where you can customize the damage and resist multipliers.

**dokebi21.SanguinePact.cfg**

| SECTION          | KEY        | DESCRIPTION                              | DEFAULT |
|------------------|------------|------------------------------------------|---------|
| DamageMultiplier | `multiplier` | Damage multiplier. 2 means deal 2x damage. | `2`       |
| ResistMultiplier | `multiplier` | Resist multiplier. 4 means take 4x damage. | `4`       |

## Notes

- Sanguine Pact cannot be toggled while in-combat.

## Future Plans

- Allow owners to configure defense and offense multipliers.
- Allow owners to configure messages.

## Credits

The V Rising Mod Community has been a great help in developing this mod. Thank you for collaborating with me on Discord.

This mod shares a lot of code from existing mods from the community, especially from the following mods.

- [Bloodcraft](https://github.com/mfoltz/Bloodcraft) by [mfoltz (zfolmt)](https://github.com/mfoltz)
- [KindredCommands](https://github.com/Odjit/KindredCommands) by [odjit](https://github.com/Odjit)
- [BloodyNotify](https://github.com/oscarpedrero/BloodyNotify) by [oscarpedrero (Trodi)](https://github.com/oscarpedrero)
