# Music
Музыкальное приложение с возможностью создать групповой сеанс прослушивания  
Разработано для дипломного проекта  

## Режим обычного прослушивания
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/0c537cc3-5c1d-4ad8-80f6-049ae063924b)


## Режим одновременного прослушивания
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/57c207d0-01dc-43ab-89af-3a2c0179d3a7)

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
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/83e6b26f-abd7-42dc-9943-6e079107bad2)

## Диаграмма прецедентов
![image](https://github.com/Grapple228/MusicApp-Final/assets/97295498/5e81bf4b-0ab7-4b3f-bb8f-8c9a47f7c4b6)

## Пояснительная записка к диплому  
[Пояснительная записка.docx](https://github.com/Grapple228/MusicApp-Final/files/11816052/default.docx)

## Презентация
Рекомендуется использовать Powerpoint версии 2019 года или выше, так как применяется переход между слайдами "Трансформация"
[Сенчуков А.В..pptx](https://github.com/Grapple228/MusicApp-Final/files/11816249/default.pptx)

