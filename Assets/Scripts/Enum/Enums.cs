public enum itemType
    {
        gold = 0,
        darkMat,
        fireMat,
        glowMat,
        grassMat,
        waterMat,
        healPotion,
        clearPotion,
        sword,
        bigSword,
        hammer,
    };
public enum State
{
    idle = 0,
    move,
    attack,
    dead,
    hited,
    skill
};
public enum mapType
{
    dark = 0,
    fire,
    miniFire,
    glow,
    grass,
    miniGrass,
    water,
    miniWater,
};
public enum type
{
    Sword = 1,
    Hammer,
    BigSword,
    Long,
    Short,
    boss,
};
public enum skillType {
    Confuse = 1,
    KnockBack,
    DarkSide,
    DarkScreen,

};