# 📚 SkinTradingApp: Система торговли игровыми скинами

🚀 Консольное приложение на C# для безопасной торговли игровыми предметами (скинами) между пользователями. Система предоставляет удобный интерфейс для покупки, продажи и управления инвентарем скинов.

GitHub License .NET 8.0 SQLite/In-memory DB

## ✨ Ключевые возможности

- 👤 **Управление пользователями**: Регистрация, авторизация, баланс
- 🎮 **Торговля скинами**: Покупка и продажа предметов между пользователями
- 🏦 **Инвентарь**: Просмотр своих скинов
- 💰 **Комиссионная система**: Автоматический расчет комиссии при сделках
- 🔄 **Интеграция с Steam**: (Имитация) передача скинов между аккаунтами
- ⚖️ **Транзакции**: История всех операций

## 🎮 Команды меню

### Гостевые команды:
| Действие | Что делает? |
|----------|-------------|
| Войти в систему | Авторизация существующего пользователя |
| Зарегистрироваться | Создание нового аккаунта |
| Просмотреть доступные скины | Показать все скины на площадке |
| Выйти из программы | Завершение работы |

### Авторизованные команды:
| Действие | Что делает? |
|----------|-------------|
| Просмотреть мой инвентарь | Показать все скины пользователя |
| Купить скин | Покупка выбранного скина у другого пользователя |
| Продать скин | Продажа скина из своего инвентаря |
| Выйти из аккаунта | Завершение текущей сессии |
| Выйти из программы | Завершение работы |

## 🗃️ База данных

### 📑 Таблицы
| Таблица | Назначение | Поля |
|---------|------------|------|
| Users | Хранит пользователей | Id (PK), Email, PasswordHash, Balance |
| Skins | Хранит скины | Id (PK), Name, Game, Rarity, Price, Condition, SteamItemId |
| InventoryItems | Связь пользователей и скинов | Id (PK), UserId (FK), SkinId (FK) |
| Transactions | История сделок | Id (PK), BuyerId (FK), SellerId (FK), SkinId (FK), Amount, Date |

### 🔗 Связи
- Один-ко-многим:
  - Users → InventoryItems: Один пользователь — много скинов в инвентаре
  - Users → Transactions (как Buyer): Один покупатель — много покупок
  - Users → Transactions (как Seller): Один продавец — много продаж
  - Skins → InventoryItems: Один скин может быть у многих пользователей (в разное время)

## 🛠️ Инициализация

База данных создается автоматически при первом запуске (in-memory). Для работы с реальной СУБД:

1. Установите .NET 8.0 SDK
2. Установите SQLite (или другую СУБД)
3. Настройте строку подключения в `appsettings.json`

## 📈 Диаграммы

### 1. 🔄 Диаграмма последовательностей (Покупка скина)

![image](https://github.com/user-attachments/assets/aea34931-7b14-44ed-a4ca-94fbcf2ee41f)


```plantuml
@startuml
actor Покупатель
participant "Консольный интерфейс" as UI
participant "TradingService" as Service
participant "SteamIntegration" as Steam

Покупатель -> UI: Выбирает скин для покупки
UI -> Service: Запрос на покупку
Service -> Steam: Проверка наличия скина
Steam --> Service: Подтверждение
Service -> Service: Проверка баланса
Service -> Service: Создание транзакции
Service -> Steam: Запрос на передачу скина
Steam --> Service: Подтверждение
Service --> UI: Результат операции
UI --> Покупатель: Уведомление о результате
@enduml
```

### 2. 🎯 Диаграмма вариантов использования

![image](https://github.com/user-attachments/assets/e4a0fc2c-6000-4972-a618-2d953e0c9f9b)

```plantuml
@startuml
left to right direction
actor Пользователь
actor Администратор
actor SteamAPI

rectangle Система {
  Пользователь --> (Купить скин)
  Пользователь --> (Продать скин)
  Пользователь --> (Управление балансом)
  
  Администратор --> (Модерация пользователей)
  Администратор --> (Просмотр статистики)
  
  SteamAPI --> (Синхронизация инвентаря)
  SteamAPI --> (Перевод скинов)
}

(Купить скин) .> (Пополнить баланс) : extends
@enduml
```

### 3. 🧱 Диаграмма классов

![image](https://github.com/user-attachments/assets/682fb850-909c-493b-b7e4-07c7282be948)

```plantuml
@startuml
class User {
  +Id: int
  +Email: string
  +PasswordHash: string
  +Balance: decimal
  +AddFunds(amount)
  +WithdrawFunds(amount)
}

class Skin {
  +Id: int
  +Name: string
  +Game: string
  +Price: decimal
  +UpdatePrice()
}

class Transaction {
  +Id: int
  +BuyerId: int
  +SellerId: int
  +SkinId: int
  +Amount: decimal
  +Process()
}

class SteamIntegration {
  +ApiKey: string
  +FetchInventory()
  +TransferSkin()
}

User "1" *-- "0..*" InventoryItem
InventoryItem "1" -- "1" Skin
User "1" -- "0..*" Transaction
Transaction "1" -- "1" Skin
SteamIntegration -- User
SteamIntegration -- Skin
@enduml
```

## 🛠️ Установка и запуск

### Требования:
- .NET 8.0 SDK
- Для работы с реальной БД: SQLite/SQL Server

### Запуск:
```bash
dotnet restore
dotnet build
dotnet run
```

## 🧩 Основные классы

### 📖 Модели
- **User**: Пользователь системы (email, пароль, баланс)
- **Skin**: Игровой предмет (название, игра, редкость, цена)
- **InventoryItem**: Связь пользователя и скина
- **Transaction**: Запись о сделке (покупатель, продавец, скин, сумма)

### 💾 Данные
- **DatabaseContext**: Контекст Entity Framework Core
- **Repositories**: Классы для работы с данными (UserRepository, SkinRepository)

### ⚙️ Сервисы
- **AuthService**: Управление пользователями и аутентификацией
- **TradingService**: Основная бизнес-логика торговли
- **InventoryService**: Управление инвентарем

### 🎬 Программа
- **ConsoleService**: Главный консольный интерфейс
- **MenuBuilder**: Построитель меню

## 📝 Заметки

1. Для реального использования необходимо:
   - Реализовать хеширование паролей
   - Добавить интеграцию с реальным Steam API
   - Настроить работу с постоянной БД (не in-memory)

2. Система комиссий:
   - При продаже скина взимается 10% комиссия
   - Комиссия рассчитывается автоматически

3. Безопасность:
   - Все транзакции выполняются в рамках одной атомарной операции
   - Проверка баланса перед сделкой
   - Валидация входных данных



Итог выполнения Программы: 

![image](https://github.com/user-attachments/assets/0aab539c-2e6c-440a-902c-3b4c5da74dc9)

![image](https://github.com/user-attachments/assets/596a17b7-3545-4b16-a05f-ad223174378a)

![image](https://github.com/user-attachments/assets/6fa5bd6c-b293-4b66-9919-a5171a895ada)

![image](https://github.com/user-attachments/assets/556218f4-b515-4b6b-b6b7-07991f86214b)

![image](https://github.com/user-attachments/assets/6634f812-2bc2-4ef8-84a5-4d0591bcbdc4)

![image](https://github.com/user-attachments/assets/8d044e40-2463-4d32-836b-682068e091d9)

![image](https://github.com/user-attachments/assets/0846d2c5-51f7-4aaf-b68e-b07649f5aff9)

![Uploading image.png…]()










