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
        static int hp = 100;
        static int maxHp = 100;
        static int baseDamage = 10;
        static List<Item> inventory = new List<Item>();


        static Random rand = new Random();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            
            GenerateRandomMap();

            while (hp > 0)
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

            int damageBonus = inventory.Sum(i => i.DamageBoost);
            int defenseBonus = inventory.Sum(i => i.DefenseBoost);

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
            Console.WriteLine($"HP: {hp}/{maxHp} | Útok: {baseDamage + damageBonus} (+{damageBonus}) | Obrana: {defenseBonus}");
            
            Console.WriteLine("\n--- INVENTÁR ---");
            if (inventory.Count == 0)
            {
                Console.WriteLine("Prázdny");
            }
            else
            {
                foreach (var item in inventory)
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
                inventory.Add(newItem);
                Console.WriteLine($"Našiel si zbraň: {newItem.Name} (Útok +{newItem.DamageBoost})!");
            }
            else if (itemType == 1) 
            {
                newItem = new Item {
                    Name = armors[rand.Next(armors.Length)],
                    DamageBoost = 0,
                    DefenseBoost = rand.Next(2, 6) 
                };
                inventory.Add(newItem);
                Console.WriteLine($"Našiel si zbroj: {newItem.Name} (Obrana +{newItem.DefenseBoost})!");
            }
            else 
            {
                int healAmount = rand.Next(15, 35);
                hp = Math.Min(maxHp, hp + healAmount);
                Console.WriteLine($"Našiel si a vypil: {consumables[rand.Next(consumables.Length)]}! Vyliečil si si {healAmount} HP.");
            }

            Console.WriteLine("\nStlač ľubovoľný kláves pre pokračovanie...");
            Console.ReadKey();
        }

        static void Fight()
        {
            int damageBonus = inventory.Sum(i => i.DamageBoost);
            int defenseBonus = inventory.Sum(i => i.DefenseBoost);

            int playerDamage = baseDamage + damageBonus;

            int enemyDamage = Math.Max(2, 20 - defenseBonus); 
            
            hp -= enemyDamage;

            Console.Clear();
            Console.WriteLine("⚔️ === BOJ === ⚔️");
            Console.WriteLine($"Zaútočil si na nepriateľa za {playerDamage} dmg.");
            Console.WriteLine($"Nepriateľ ti vrátil úder. Vďaka tvojej obrane (+{defenseBonus}) ti ubral iba {enemyDamage} HP.");
            
            Console.WriteLine("\nStlač ľubovoľný kláves...");
            Console.ReadKey();
        }
    }

    class Item
    {
        public string Name { get; set; }
        public int DamageBoost { get; set; }
        public int DefenseBoost { get; set; }
    }
}
