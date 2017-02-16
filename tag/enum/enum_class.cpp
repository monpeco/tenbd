#include<iostream>
#include<cstdio>
using namespace std;

//By default => ROJO = 0 VERDE = 1 AZUL = 2 AMARILLO = 3
enum class Colors
{
    ROJO,
    VERDE,    
    AZUL,
    AMARILLO,
};

struct Days 
{
   enum type
   {
      Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday
   };
};

int main()
{

if (0 == (int)Colors::ROJO)
    cout << "Colors::ROJO: ["<< (int)Colors::ROJO << "]" <<endl;
    
Days::type day = Days::Wednesday;
if ((day >= Days::Monday) && (day <= Days::Friday))
    cout << "Day of the week: ["<< day << "]" <<endl;

    /*output
        g++ -std=c++11 enum_class.cpp -o enum_class.exe
        Colors::ROJO: [0]
        Day of the week: [3]
    */
    return 0;
}
/* another way
struct Days 
{
   enum type
   {
      Saturday,Sunday,Tuesday,Wednesday,Thursday,Friday
   };
};

Days::type day = Days::Saturday;
if (day == Days::Saturday)*/

//g++ -std=c++11 enum_class.cpp -o enum_class.exe