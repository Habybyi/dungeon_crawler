using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonCrawler
{
    class Program
    {
        static int mapWidth = 15;
        static int mapHeight = 8;
        static char[,] map;
        static int playerX, playerY;
        static Random rand = new Random();
        static Player player;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            
            try
            {
                player = new Player("Hrdina", 100, 10);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kritická chyba: {ex.Message}");
                return;
            }
            finally
            {
                GenerateRandomMap();
            }

            while (player.HP > 0)
            {
                Draw();
                HandleInput();
            }

            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("  GAME OVER! Zomrel si v dungeone.       ");
            Console.WriteLine("========================================");
        }

        static void GenerateRandomMap()
        {
            map = new char[mapHeight, mapWidth];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (y == 0 || y == mapHeight - 1 || x == 0 || x == mapWidth - 1)
                    {
                        map[y, x] = '#';
                    }
                    else
                    {
                        int chance = rand.Next(100);
                        if (chance < 8) map[y, x] = 'E';        
                        else if (chance < 13) map[y, x] = 'I'; 
                        else if (chance < 18) map[y, x] = '#'; 
                        else map[y, x] = '.';                  
                    }
                }
            }

            do
            {
                playerX = rand.Next(1, mapWidth - 1);
                playerY = rand.Next(1, mapHeight - 1);
            } while (map[playerY, playerX] != '.');
        }

        static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("=== NÁHODNÝ DUNGEON CRAWLER ===");

            int damageBonus = player.Inventory.Sum(i => i.DamageBoost);
            int defenseBonus = player.Inventory.Sum(i => i.DefenseBoost);

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (x == playerX && y == playerY) 
                        Console.Write("@");
                    else 
                        Console.Write(map[y, x]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n--- TVOJE STATY ---");
            Console.WriteLine($"HP: {player.HP}/{player.MaxHP} | Útok: {player.BaseDamage + damageBonus} (+{damageBonus}) | Obrana: {defenseBonus}");
            
            Console.WriteLine("\n--- INVENTÁR ---");
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Prázdny");
            }
            else
            {
                foreach (var item in player.Inventory)
                {
                    string stats = "";
                    if (item.DamageBoost > 0) stats += $"[Útok +{item.DamageBoost}] ";
                    if (item.DefenseBoost > 0) stats += $"[Obrana +{item.DefenseBoost}] ";
                    Console.WriteLine($"- {item.Name} {stats}");
                }
            }

            Console.WriteLine("\nPohyb: WASD | Koniec: Q");
        }

        static void HandleInput()
        {
            var key = Console.ReadKey(true).Key;
            int newX = playerX, newY = playerY;

            if (key == ConsoleKey.W) newY--;
            if (key == ConsoleKey.S) newY++;
            if (key == ConsoleKey.A) newX--;
            if (key == ConsoleKey.D) newX++;
            if (key == ConsoleKey.Q) Environment.Exit(0);

            char target = map[newY, newX];

            if (target == '#') return; 

            if (target == 'E') {
                Fight();
                map[newY, newX] = '.'; 
            }
            else if (target == 'I') {
                PickUpRandomItem();
                map[newY, newX] = '.';
            }

            playerX = newX;
            playerY = newY;
        }

        static void PickUpRandomItem()
        {
            string[] weapons = { "Hrdzavý meč", "Oceľová dýka", "Bojová sekera", "Palcát skazy" };
            string[] armors = { "Kožená vesta", "Krúžkové brnenie", "Plátový pancier", "Mystický plášť" };
            string[] consumables = { "Malý liečivý lektvar", "Veľký liečivý lektvar" };

            int itemType = rand.Next(3); 
            Item newItem = null;

            Console.Clear();

            if (itemType == 0) 
            {
                newItem = new Item {
                    Name = weapons[rand.Next(weapons.Length)],
                    DamageBoost = rand.Next(3, 10),
                    DefenseBoost = 0
                };
                player.Inventory.Add(newItem);
                Console.WriteLine($"Našiel si zbraň: {newItem.Name} (Útok +{newItem.DamageBoost})!");
            }
            else if (itemType == 1) 
            {
                newItem = new Item {
                    Name = armors[rand.Next(armors.Length)],
                    DamageBoost = 0,
                    DefenseBoost = rand.Next(2, 6) 
                };
                player.Inventory.Add(newItem);
                Console.WriteLine($"Našiel si zbroj: {newItem.Name} (Obrana +{newItem.DefenseBoost})!");
            }
            else 
            {
                int healAmount = rand.Next(15, 35);
                player.HP = Math.Min(player.MaxHP, player.HP + healAmount);
                Console.WriteLine($"Našiel si a vypil: {consumables[rand.Next(consumables.Length)]}! Vyliečil si si {healAmount} HP.");
            }

            Console.WriteLine("\nStlač ľubovoľný kláves pre pokračovanie...");
            Console.ReadKey();
        }

        static void Fight()
        {
            GameCharacter enemy = new Enemy("Zlý Goblin", 30, 20);

            int damageBonus = player.Inventory.Sum(i => i.DamageBoost);
            int playerDamage = player.BaseDamage + damageBonus;

            Console.Clear();
            Console.WriteLine("⚔️ === BOJ === ⚔️");
            
            enemy.TakeDamage(playerDamage);
            player.TakeDamage(enemy.BaseDamage);

            Console.WriteLine("\nStlač ľubovoľný kláves...");
            Console.ReadKey();
        }
    }

    public abstract class GameCharacter
    {
        private int _hp;
        private int _baseDamage;

        public int HP
        {
            get { return _hp; }
            set { _hp = value < 0 ? 0 : value; }
        }

        public int BaseDamage
        {
            get { return _baseDamage; }
            set { _baseDamage = value < 0 ? 0 : value; }
        }

        public string Name { get; set; }

        public GameCharacter(string name, int hp, int baseDamage)
        {
            Name = name;
            HP = hp;
            BaseDamage = baseDamage;
        }

        public virtual void TakeDamage(int damage)
        {
            HP -= damage;
            Console.WriteLine($"{Name} dostal {damage} dmg. Zostáva mu {HP} HP.");
        }
    }

    public class Player : GameCharacter
    {
        public int MaxHP { get; set; } = 100;
        public List<Item> Inventory { get; set; } = new List<Item>();

        public Player(string name, int hp, int baseDamage) : base(name, hp, baseDamage)
        {
        }

        public override void TakeDamage(int damage)
        {
            int defenseBonus = Inventory.Sum(i => i.DefenseBoost);
            int actualDamage = Math.Max(2, damage - defenseBonus);
            
            HP -= actualDamage;
            Console.WriteLine($"Nepriateľ ti vrátil úder. Vďaka tvojej obrane (+{defenseBonus}) ti ubral iba {actualDamage} HP.");
        }
    }

    public class Enemy : GameCharacter
    {
        public Enemy(string name, int hp, int baseDamage) : base(name, hp, baseDamage)
        {
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int DamageBoost { get; set; }
        public int DefenseBoost { get; set; }
    }
}
