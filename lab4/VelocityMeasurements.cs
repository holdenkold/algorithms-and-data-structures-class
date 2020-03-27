
namespace ASD
{
using System;

public class VelocityMeasurements : System.MarshalByRefObject
    {

    /// <summary>
    /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w momencie wypadku.
    /// </summary>
    /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
    /// <param name="isBraking">Tablica zwracająca informację dla każdego z pomiarów z tablicy measurements informację bool czy dla sekwencji dającej
    /// minimalną prędkość wynikową traktować dany pomiar jako hamujący (true) przy przyspieszający (false)</param>
    /// <returns>Krotka z informacjami o najniższej i najwyższej możliwej prędkości w momencie wypadku, numer pomiaru (nr) to w tym przypadku zawsze rozmiar tablicy pomiarów</returns>
    public (int minVelocity, int maxVelocity, int nr) FinalVelocities(int[] measurements, out bool[] isBraking)
        {
        isBraking = null;
        return (-1,-1,-1);
        }

    /// <summary>
    /// Metoda zwraca możliwą minimalną i maksymalną wartość prędkości samochodu w trakcie całego okresu trwania podróży.
    /// </summary>
    /// <param name="measurements">Tablica zawierające wartości pomiarów urządzenia zainstalowanego w aucie Mateusza</param>
    /// <returns>Krotka z informacjami o najniższej i najwyższej możliwej prędkości na trasie oraz informacją o numerze pomiaru dla którego najniższe prędkość może być osięgnięta</returns>
    public (int minVelocity, int maxVelocity, int nr) JourneyVelocities(int[] measurements)
        {
        return (-1,-1,-1);
        }

    }

}
