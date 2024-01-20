using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Добро пожаловать, воин!");
        Console.Write("Назови себя:\n> ");
        string playerName = Console.ReadLine();

        Player player = new Player(playerName, 100, 100);
        player.Weapon = new Weapon("Двойные клинки", 40);
        player.AidKit = new Aid("Средняя аптечка", 50);

        Console.WriteLine($"Ваше имя **{player.Name}**!");
        Console.WriteLine($"Вам было ниспослано оружие **{player.Weapon.Name} ({player.Weapon.Damage})**, а также **{player.AidKit.Name} ({player.AidKit.HealAmount}hp)**.");
        Console.WriteLine($"У вас {player.CurrentHealth}hp и {player.Score} очков.\n");

        Random rnd = new Random();
        Game game = new Game(player, rnd);
        game.Start();
    }
}

public class Player
{
    public string Name { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public Aid AidKit { get; set; }
    public Weapon Weapon { get; set; }
    public int Score { get; private set; }
    public Enemy Enemy { get; set; }

    public Player(string name, int maxHealth, int currentHealth)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Score = 0;
    }

    public void Heal()
    {
        if (AidKit != null)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + AidKit.HealAmount);
            Console.WriteLine($"**{Name}** использовал аптечку");
            Console.WriteLine($"У противника {Enemy.CurrentHealth}hp, у вас {CurrentHealth}hp");
            AidKit = null;
        }
        else
        {
            Console.WriteLine("У вас нет аптечки!");
        }
    }

    public void Attack(Enemy enemy)
    {
        if (Weapon != null)
        {
            Console.WriteLine($"**{Name}** ударил противника **{enemy.Name}**");
            enemy.TakeDamage(Weapon.Damage);
            Console.WriteLine($"У противника {enemy.CurrentHealth}hp, у вас {CurrentHealth}hp");
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Console.WriteLine($"**{Name}** пал в бою!");
        }
    }

    public void AddScore(int score)
    {
        Score += score;
    }
}

public class Enemy
{
    public string Name { get; private set; }
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public Weapon Weapon { get; private set; }

    public Enemy(string name, int maxHealth, Weapon weapon)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Weapon = weapon;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Console.WriteLine($"**{Name}** был повержен!");
        }
        else
        {
            Console.WriteLine($"У противника **{Name}** осталось {CurrentHealth}hp.");
        }
    }

    public void Attack(Player player)
    {
        player.TakeDamage(Weapon.Damage);
        Console.WriteLine($"Противник **{Name}** ударил вас!");
        Console.WriteLine($"У противника {CurrentHealth}hp, у вас {player.CurrentHealth}hp");
    }
}

public class Aid
{
    public string Name { get; private set; }
    public int HealAmount { get; private set; }

    public Aid(string name, int healAmount)
    {
        Name = name;
        HealAmount = healAmount;
    }
}

public class Weapon
{
    public string Name { get; private set; }
    public int Damage { get; private set; }

    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }
}

public class Game
{
    private Player Player { get; set; }
    private Random Random { get; set; }

    public Game(Player player, Random random)
    {
        Player = player;
        Random = random;
    }

    public void Start()
    {
        while (Player.CurrentHealth > 0)
        {
            GenerateRandomEnemyAndWeapon();

            while (Player.Enemy.CurrentHealth > 0 && Player.CurrentHealth > 0)
            {
                Console.WriteLine("Что вы будете делать?");
                Console.WriteLine("1. Ударить");
                Console.WriteLine("2. Пропустить ход");
                Console.WriteLine("3. Использовать аптечку");
                Console.Write("> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Player.Attack(Player.Enemy);
                        if (Player.Enemy.CurrentHealth > 0)
                        {
                            Player.Enemy.Attack(Player);
                        }
                        break;
                    case "2":
                        Player.Enemy.Attack(Player);
                        break;
                    case "3":
                        Player.Heal();
                        if (Player.Enemy.CurrentHealth > 0)
                        {
                            Player.Enemy.Attack(Player);
                        }
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }

                if (Player.Enemy.CurrentHealth <= 0)
                {
                    Player.AddScore(10);
                    Console.WriteLine($"**{Player.Name}** убил врага **{Player.Enemy.Name}** и получил 10 очков!");
                    Console.WriteLine($"Текущий счет: {Player.Score} очков");
                }


                if (Player.CurrentHealth <= 0)
                {
                    Console.WriteLine("Игра окончена! Ваш счет: " + Player.Score);
                    break;
                }
            }
        }
    }

    private void GenerateRandomEnemyAndWeapon()
    {
        var enemyNames = new List<string> { "Варвар", "Гоблин", "Орк", "Некромант", "Дракон", "Скелетон", "Ведьма", "Зомби" };
        var weaponNames = new List<string> { "Экскалибур", "Фламберг", "Катана", "Кинжал", "Молот", "Палица", "Кувалда", "Лук" };

        string enemyName = enemyNames[Random.Next(enemyNames.Count)];
        int enemyHealth = Random.Next(30, 100);
        string weaponName = weaponNames[Random.Next(weaponNames.Count)];
        int weaponDamage = Random.Next(15, 40);

        Enemy enemy = new Enemy(enemyName, enemyHealth, new Weapon(weaponName, weaponDamage));
        Player.Enemy = enemy;
        Console.WriteLine($"**{Player.Name}** встречает врага **{enemy.Name} ({enemy.CurrentHealth}hp)**, у врага на поясе сияет оружие **{enemy.Weapon.Name} ({enemy.Weapon.Damage})**");
    }
}
