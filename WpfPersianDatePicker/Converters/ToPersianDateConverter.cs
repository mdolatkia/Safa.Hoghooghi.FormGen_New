﻿using System;
using System.Globalization;
using System.Windows.Data;
using WpfPersianDatePicker.DateHelper;

namespace WpfPersianDatePicker.Converters
{
    /// <summary>
    /// IValueConverter class for converting Gregorian Date to Persian Date.
    /// </summary>
    /// <author>
    ///   <name>Vahid Nasiri</name>
    ///   <email>vahid_nasiri@yahoo.com</email>
    /// </author>    
    public class ToPersianDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            DateTime? date = value.DateTimeTryParse();
            if (!date.HasValue) return null;

            int year, month, day;
            if (PDateHelper.GregorianToHijri(
                date.Value.Year,
                date.Value.Month,
                date.Value.Day,
                out year, out month, out day))
            {
                return string.Format("{0}/{1}/{2}", year, month.ToString("00"), day.ToString("00"));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
