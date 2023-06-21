# Music
Музыкальное приложение с возможностью создать групповой сеанс прослушивания  
## Режим обычного прослушивания
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/8e66d1ae-c4a5-4b63-b382-2aee4d99cbf5)

## Режим одновременного прослушивания
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/63b1ee28-ded4-45dc-a515-58a9f0402168)

# Средства разработки
- C# 11, NET CORE 8
- Mongo Db
- ASP NET CORE + SignalR + RabbitMQ + Swagger
- WPF

# Стартовое состояние
По умолчанию в системе создается администратор с ID ffffffff-ffff-ffff-ffff-ffffffffffff.  
При удалении пользователя, он будет создан снова после перезапуска Identity сервиса  
Логин: Admin  
Пароль: password

# Сервисы
В системе определено 9 сервисов:
- Gateway (8000)
- Identity (8001)
- Users (8002)
- Tracks (8003)
- Artists (8004)
- Albums (8005)
- Playlists (8006)
- Genres (8007)
- Images (8008)

## Структура системы
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/264acdf3-aea4-4044-ab80-659c39043d5b)


## Диаграмма прецедентов
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/060147ef-5c8e-4d12-bdaa-380637e4a6d6)

