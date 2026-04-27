using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonCrawler
{
    class Program
    {
        // Hráč a jeho vlastnosti
        static int playerX = 1, playerY = 1;
        static int hp = 100;
        static List<Item> inventory = new List<Item>();

        // Mapa ( # = stena, . = cesta, E = nepriateľ, I = item )
        static string[] map = {
            "##########",
            "#........#",
            "#.I..E...#",
            "#........#",
            "#.E....I.#",
            "##########"
        };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            while (hp > 0)
            {
                Draw();
                HandleInput();
            }
            Console.Clear();
            Console.WriteLine("Game Over! Zomrel si v dungeone.");
        }

        static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("--- DUNGEON CRAWLER ---");
            
            // Výpočet bonusov z inventára
            int damageBonus = inventory.Sum(i => i.DamageBoost);
            
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (x == playerX && y == playerY) Console.Write("@"); // Hráč
                    else Console.Write(map[y][x]);
                }
                Console.WriteLine();
            }

            Console.WriteLine($"\nHP: {hp} | Sila: {10 + damageBonus}");
            Console.WriteLine("Inventár: " + (inventory.Count == 0 ? "Prázdny" : string.Join(", ", inventory.Select(i => i.Name))));
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

            char target = map[newY][newX];

            if (target == '#') return; // Stena, nepohneme sa

            if (target == 'E') {
                Fight();
                RemoveObject(newX, newY);
            }
            else if (target == 'I') {
                PickUpItem();
                RemoveObject(newX, newY);
            }

            playerX = newX;
            playerY = newY;
        }

        static void PickUpItem()
        {
            var sword = new Item { Name = "Meč", DamageBoost = 5 };
            inventory.Add(sword);
        }

        static void Fight()
        {
            int damageDealt = 10 + inventory.Sum(i => i.DamageBoost);
            hp -= 20; // Nepriateľ ti vždy uberie 20 HP
            Console.Clear();
            Console.WriteLine($"Bojoval si! Spôsobil si {damageDealt} dmg a stratil si 20 HP.");
            Console.WriteLine("Stlač ľubovoľný kláves...");
            Console.ReadKey();
        }

        static void RemoveObject(int x, int y)
        {
            char[] row = map[y].ToCharArray();
            row[x] = '.';
            map[y] = new string(row);
        }
    }

    class Item
    {
        public string Name { get; set; }
        public int DamageBoost { get; set; }
    }
}