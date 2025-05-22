using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Input;

namespace AppCalculadora.View
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private double _valorAtual = 0;
        private double _primeiroNumero = 0;
        private string _operador = "";
        private bool _novoNumero= true;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        public ICommand Numero { get; private set; }
        public ICommand Operador{ get; private set; }
        public ICommand Limpar { get; private set; }
        public ICommand Igualdade { get; private set; }
        public ICommand Decimal { get; private set; }
        public ICommand Backspace { get; private set; }

        public MainPageViewModel()
        {
            Numero = new Command<string>(OnNumeroClicked);
            Operador = new Command<string>(OnOperadorClicked);
            Limpar = new Command(OnLimparClicked);
            Igualdade = new Command(OnIgualdadeClicked);
            Decimal = new Command(OnDecimalClicked);
            Backspace = new Command(OnBackspaceClicked);
        }

        private void OnNumeroClicked(string number)
        {
            if (_novoNumero)
            {
                DisplayText = number;
                _novoNumero = false;
            }
            else
            {
                DisplayText += number;
            }
        }

        private void OnOperadorClicked(string op)
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.CurrentCulture, out double numeroParaOperacao))
            {
                if (!string.IsNullOrEmpty(_operador) && !_novoNumero)
                {
                    CalcularResultadoIntermediario(numeroParaOperacao);
                }
                else
                {
                    _primeiroNumero = numeroParaOperacao;
                }

                _operador = op; 
                _novoNumero = true;
            }
        }
        private void CalcularResultadoIntermediario(double segundoNumero)
        {
            double resultado = 0;
            bool erroDivisaoPorZero = false;

            switch (_operador)
            {
                case "+":
                    resultado = _primeiroNumero + segundoNumero;
                    break;
                case "-":
                    resultado = _primeiroNumero - segundoNumero;
                    break;
                case "X":
                    resultado = _primeiroNumero * segundoNumero;
                    break;
                case "/":
                    if (segundoNumero != 0)
                    {
                        resultado = _primeiroNumero / segundoNumero;
                    }
                    else
                    {
                        DisplayText = "Erro";
                        erroDivisaoPorZero = true;
                    }
                    break;
            }

            if (!erroDivisaoPorZero)
            {
                DisplayText = resultado.ToString("G15", CultureInfo.CurrentCulture);
                _primeiroNumero = resultado;
            }
        }

        private void OnLimparClicked()
        {
            DisplayText = "0";
            _valorAtual = 0;
            _primeiroNumero = 0;
            _operador = "";
            _novoNumero = true;
        }

        private void OnIgualdadeClicked()
        {
            if (!string.IsNullOrEmpty(_operador)) 
            {
                if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.CurrentCulture, out double numeroAtualParaCalculo))
                {
                    CalcularResultadoIntermediario(numeroAtualParaCalculo);
                    double resultado = 0;
                    bool erroDivisaoPorZero = false;

                    switch (_operador)
                    {
                        case "+":
                            resultado = _primeiroNumero + _valorAtual;
                            break;
                        case "-":
                            resultado = _primeiroNumero - _valorAtual;
                            break;
                        case "X":
                            resultado = _primeiroNumero * _valorAtual;
                            break;
                        case "/":
                            if (_valorAtual != 0)
                            {
                                resultado = _primeiroNumero / _valorAtual;
                            }
                            else
                            {
                                DisplayText = "Erro";
                                erroDivisaoPorZero = true;
                            }
                            break;
                    }

                    if (!erroDivisaoPorZero)
                    {
                        DisplayText = resultado.ToString();
                    }
                    _primeiroNumero = resultado; 
                    _operador = "";
                    _novoNumero = true;  
                }
            }
        }

        private void OnDecimalClicked()
        {
            if (_novoNumero)
            {
                DisplayText = "0" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator; 
                _novoNumero = false;
            }
            else if (!DisplayText.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) 
            {
                DisplayText += CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            }
        }

        private void OnBackspaceClicked()
        {
            if (_displayText.Length > 1)
            {
                DisplayText = _displayText.Substring(0, _displayText.Length - 1);
            }
            else
            {
                DisplayText = "0";
                _novoNumero = true;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
