#include "complejo.h"
using namespace std;

// constructor por defecto
complejo::complejo(void)
{
	real = 0.0;
	imag = 0.0;
}

// constructor general
complejo::complejo(double re, double im)
{
	real = re;
	imag = im;
}

// constructor de copia
complejo::complejo(const complejo& c)
{
	real = c.real;
	imag = c.imag;
}

// función miembro SetData()
void complejo::SetData(void)
{
	cout << "Introduzca el valor real del complejo: ";
	cin >> real;
	cout << "Introduzca el valor imaginario del complejo: ";
	cin >> imag;
}
void complejo::SetReal(double re)
{
	real = re;
}
void complejo::SetImag(double im)
{
	imag = im;
}

// operador miembro + sobrecargado
complejo complejo::operator+ (const complejo &a)
{
	complejo suma;
	suma.real = real + a.real;
	suma.imag = imag + a.imag;
	return suma;
}
// operador miembro - sobrecargado
complejo complejo::operator- (const complejo &a)
{
	complejo resta;
	resta.real = real - a.real;
	resta.imag = imag - a.imag;
	return resta;
}
// operador miembro * sobrecargado
complejo complejo::operator* (const complejo &a)
{
	complejo producto;
	producto.real = real*a.real - imag*a.imag;
	producto.imag = real*a.imag + a.real*imag;
	return producto;
}
// operador miembro / sobrecargado
complejo complejo::operator/ (const complejo &a)
{
	complejo cociente;
	double d = a.real*a.real + a.imag*a.imag;
	cociente.real = (real*a.real + imag*a.imag)/d;
	cociente.imag = (-real*a.imag + imag*a.real)/d;
	return cociente;
}

// operador miembro de asignación sobrecargado
complejo& complejo::operator= (const complejo &a)
{
	real = a.real;
	imag = a.imag;
	return (*this);
}

// operador friend de test de igualdad sobrecargado
int operator== (const complejo& a, const complejo& b)
{
	if (a.real==b.real && a.imag==b.imag)
		return 1;
	else
		return 0;
}
// operador friend de test de desigualdad sobrecargado
int operator!= (const complejo& a, const complejo& b)
{
	if (a.real!=b.real || a.imag!=b.imag)
		return 1;
	else
		return 0;
}

// operador friend << sobrecargado
ostream& operator << (ostream& co, const complejo &a)
{
	co << a.real;
	std::ios_base::fmtflags fl = co.setf(ios::showpos);
	co << a.imag << "i";
	co.flags(fl);
	return co;
}

