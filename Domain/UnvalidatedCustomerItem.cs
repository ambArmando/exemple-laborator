﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1PSSC.Domain
{
    public record UnvalidatedCustomerItem(string itemCode, string itemQuantity, string address, string paid);
}
