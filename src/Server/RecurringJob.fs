module Server.RecurringJob

let GetAuctionData()=

    //sleep for a day

    //делаем запрос вида select min startyear, max endyear, distict maker
    //по этому запросу получаем байки с апи для всех производителей = 12 запросов (набор 1)
    //далее берем select distinct model bikeforuser join bikemodel on ..  чтобы получить только имена байков (набор 2)
    //по каждой модели в наборе 2 нужно выбрать из набора 1 соответствие и посмотреть, нету ли их уже в данных аукционов
    //взять какой-то auct data ключи чтобы не перезаписывать
    //если нету то сохранить

    ()