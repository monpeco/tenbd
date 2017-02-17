#include <iostream>
#include <string.h>
using namespace std;

inline void string_copy(char *copia, const char *original)
{
    strcpy(copia, original);
}

inline void string_copy(char *copia, const char *original, const int longitud)
{
    strncpy(copia, original, longitud);
}

static char string_a[20], string_b[20];

int main(void)
{
    string_copy(string_a, "Aquello");
    string_copy(string_b, "Esto es una cadena", 4);
    cout << string_b << " y " << string_a << endl;
    /*output
        Esto y Aquello
    */
    return 0;
}