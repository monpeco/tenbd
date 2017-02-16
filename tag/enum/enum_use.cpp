#include<iostream>
#include<cstdio>
using namespace std;

//By default => ROJO = 0 VERDE = 1 AZUL = 2 AMARILLO = 3
enum Colors {ROJO, VERDE, AZUL, AMARILLO};

//Also can have another value
enum Farben {ROT = 3, GRUN = 5, BLAU = 7, GELB};

int main()
{
    Colors color = ROJO;
    cout << "color: [" << color << "]" << endl;
    
    Farben farbe = GRUN;
    Farben farbe2 = GELB;
    cout << "farbe: [" << farbe << "]" << endl;    
    cout << "andere farbe: [" << farbe2 << "]" << endl;    
    
    /*output
        color: [0]
        farbe: [5]
        andere farbe: [8] //Not explicit
    */
    return 0;
}

