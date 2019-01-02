/**
 * Copyright (C) 2017-2019 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 * @author: From ndn-cxx security https://github.com/named-data/ndn-cxx/blob/master/src/security/validator-null.hpp
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * A copy of the GNU Lesser General Public License is in the file COPYING.
 */

package net.named_data.jndn.security;

import net.named_data.jndn.security.v2.CertificateFetcherOffline;
import net.named_data.jndn.security.v2.ValidationPolicyAcceptAll;
import net.named_data.jndn.security.v2.Validator;

/**
 * A ValidatorNull extends Validator with an "accept-all" policy and an offline
 * certificate fetcher.
 */
public class ValidatorNull extends Validator {
  public  ValidatorNull()
  {
    super(new ValidationPolicyAcceptAll(), new CertificateFetcherOffline());
  }
}
